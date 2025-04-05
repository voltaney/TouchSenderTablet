using System.Net.Sockets;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Microsoft.Extensions.Logging;

using TouchSenderReceiver.Helpers;

using TouchSenderTablet.GUI.Contracts.Services;
using TouchSenderTablet.GUI.Helpers;
using TouchSenderTablet.GUI.Models;

namespace TouchSenderTablet.GUI.ViewModels;

public partial class MainViewModel : ObservableRecipient
{
    private readonly ITouchReceiverSettingsService _touchReceiverSettingsService;
    private readonly ITouchReceiverService _touchReceiverService;
    private readonly ITouchReceiverCanvasService _touchReceiverCanvasService;
    private readonly ILogger _logger;
    private readonly TouchReceiverServiceOptions _serviceOptions;
    private readonly TouchReceiverScreenOptions _screenOptions;
    private Task? _touchReceiverTask;

    [ObservableProperty]
    public partial string IpAddresses { get; set; }

    public int PortNumber
    {
        get => _serviceOptions.PortNumber;
        set => SetProperty(_serviceOptions.PortNumber, value, _serviceOptions, (o, p) => o.PortNumber = p);
    }

    public int HorizontalSensitivity
    {
        get => _serviceOptions.HorizontalSensitivity;
        set => SetProperty(_serviceOptions.HorizontalSensitivity, value, _serviceOptions, (o, p) => o.HorizontalSensitivity = p);
    }

    public int VerticalSensitivity
    {
        get => _serviceOptions.VerticalSensitivity;
        set => SetProperty(_serviceOptions.VerticalSensitivity, value, _serviceOptions, (o, p) => o.VerticalSensitivity = p);
    }

    public bool LeftClickWhileTouched
    {
        get => _serviceOptions.LeftClickWhileTouched;
        set => SetProperty(_serviceOptions.LeftClickWhileTouched, value, _serviceOptions, (o, p) => o.LeftClickWhileTouched = p);
    }

    public bool IsTouchReceiverCanvasEnabled
    {
        get => _screenOptions.IsTouchReceiverCanvasEnabled;
        set => SetProperty(_screenOptions.IsTouchReceiverCanvasEnabled, value, _screenOptions, (o, p) => o.IsTouchReceiverCanvasEnabled = p);
    }

    public bool IsTouchInputsSettingsExpanded
    {
        get => _screenOptions.IsTouchInputsSettingsExpanded;
        set => SetProperty(_screenOptions.IsTouchInputsSettingsExpanded, value, _screenOptions, (o, p) => o.IsTouchInputsSettingsExpanded = p);
    }

    #region Monitor Settings
    public int TouchCircleSize { get; } = 16;
    private readonly int _maxCanvasSize = 400;
    private static readonly int s_defaultCanvasSize = 220;
    private static readonly int s_canvasFps = 100;
    #endregion

    #region Monitor Properties
    [ObservableProperty]
    public partial int CanvasWidth { get; set; } = s_defaultCanvasSize;
    [ObservableProperty]
    public partial int CanvasHeight { get; set; } = s_defaultCanvasSize;
    [ObservableProperty]
    public partial int TouchCircleX { get; set; }
    [ObservableProperty]
    public partial int TouchCircleY { get; set; }
    [ObservableProperty]
    public partial bool IsDataReceived { get; set; } = false;
    [ObservableProperty]
    public partial int DroppedPayloadCount { get; set; }
    [ObservableProperty]
    public partial bool IsPayloadDropped { get; set; } = false;
    #endregion

    public MainViewModel(ITouchReceiverSettingsService touchReceiverSettingsService, ITouchReceiverService touchReceiverService, ITouchReceiverCanvasService touchReceiverCanvasService, ILogger<MainViewModel> logger)
    {
        // DI
        _touchReceiverSettingsService = touchReceiverSettingsService;
        _touchReceiverService = touchReceiverService;
        _touchReceiverCanvasService = touchReceiverCanvasService;
        _logger = logger;

        // Initialize
        SetInitialCanvas();
        _touchReceiverCanvasService.InitializeCanvas(_maxCanvasSize, TouchCircleSize, s_defaultCanvasSize);
        _touchReceiverCanvasService.SetUpdateHandler((canvasSize, touchCirclePosition, droppedPayloadCount) =>
        {
            IsDataReceived = true;
            CanvasWidth = (int)canvasSize.Width;
            CanvasHeight = (int)canvasSize.Height;
            TouchCircleX = (int)touchCirclePosition.X;
            TouchCircleY = (int)touchCirclePosition.Y;
            DroppedPayloadCount = droppedPayloadCount;
            IsPayloadDropped = droppedPayloadCount > 0;
        });

        _serviceOptions = _touchReceiverSettingsService.ServiceOptions;
        _screenOptions = _touchReceiverSettingsService.ScreenOptions;

        var candidateIpAddresses = NetworkHelper.GetAllLocalIPv4().ToList();
        IpAddresses = candidateIpAddresses.Count > 0 ? string.Join(" / ", NetworkHelper.GetAllLocalIPv4()) : "Unknown".GetLocalized();
    }

    private void SetInitialCanvas()
    {
        TouchCircleX = (CanvasWidth - TouchCircleSize) / 2;
        TouchCircleY = (CanvasHeight - TouchCircleSize) / 2;
    }

    public async Task SaveOptions()
    {
        await _touchReceiverSettingsService.SaveTouchReceiverServiceOptionsAsync(_serviceOptions);
        await _touchReceiverSettingsService.SaveTouchReceiverScreenOptionsAsync(_screenOptions);
    }

    [RelayCommand(CanExecute = nameof(CanStartTouchReceiverService))]
    private async Task StartTouchReceiverService(CancellationToken token)
    {
        _touchReceiverService.SetOptions(_serviceOptions);
        _touchReceiverTask = _touchReceiverService.StartAsync(token);
        // 約100fpsで描画
        if (IsTouchReceiverCanvasEnabled)
        {
            _touchReceiverCanvasService.Start(new TimeSpan(0, 0, 0, 0, 1000 / s_canvasFps));
        }

        StopTouchReceiverServiceCommand.NotifyCanExecuteChanged();
        try
        {
            await _touchReceiverTask;
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("TouchReceiverService is canceled");
        }
        catch (SocketException e)
        {
            _logger.LogError(e, "SocketException occurred");
            WeakReferenceMessenger.Default.Send(new ShowErrorDialogMessage()
            {
                Title = "NetworkError".GetLocalized(),
                Error = e,
                Message = "NetworkErrorMessage".GetLocalized(),
            });
        }
        catch (FormatException e)
        {
            _logger.LogError(e, "FormatException occurred");
            WeakReferenceMessenger.Default.Send(new ShowErrorDialogMessage()
            {
                Title = "InvalidDataReceptionError".GetLocalized(),
                Error = e,
                Message = "InvalidDataReceptionErrorMessage".GetLocalized(),
            });
        }
        finally
        {
            IsDataReceived = false;
        }
        StartTouchReceiverServiceCommand.NotifyCanExecuteChanged();
        StopTouchReceiverServiceCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand(CanExecute = nameof(CanStopTouchReceiverService))]
    private void StopTouchReceiverService()
    {
        StartTouchReceiverServiceCommand.Cancel();
        _touchReceiverCanvasService.Stop();
        IsDataReceived = false;
        IsPayloadDropped = false;
    }
    private bool CanStartTouchReceiverService() => !IsTouchReceiverServiceRunning();
    private bool CanStopTouchReceiverService() => IsTouchReceiverServiceRunning();

    private bool IsTouchReceiverServiceRunning()
    {
        return !_touchReceiverTask?.IsCompleted ?? false;
    }
}
