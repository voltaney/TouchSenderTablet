using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using TouchSenderReceiver.Helpers;

using TouchSenderTablet.GUI.Contracts.Services;
using TouchSenderTablet.GUI.Models;

namespace TouchSenderTablet.GUI.ViewModels;

public partial class MainViewModel : ObservableRecipient
{
    private readonly ITouchReceiverSettingsService _touchReceiverSettingsService;
    private readonly ITouchReceiverService _touchReceiverService;
    private readonly ITouchReceiverCanvasService _touchReceiverCanvasService;
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
    #endregion

    #region Monitor Properties
    private static readonly int s_defaultCanvasWidth = 220;
    private static readonly int s_canvasFps = 100;
    [ObservableProperty]
    public partial int CanvasWidth { get; set; } = s_defaultCanvasWidth;
    [ObservableProperty]
    public partial int CanvasHeight { get; set; } = s_defaultCanvasWidth;
    [ObservableProperty]
    public partial int TouchCircleX { get; set; }
    [ObservableProperty]
    public partial int TouchCircleY { get; set; }
    [ObservableProperty]
    public partial bool IsDataReceived { get; set; } = false;
    #endregion

    public MainViewModel(ITouchReceiverSettingsService touchReceiverSettingsService, ITouchReceiverService touchReceiverService, ITouchReceiverCanvasService touchReceiverCanvasService)
    {
        _touchReceiverSettingsService = touchReceiverSettingsService;
        _touchReceiverService = touchReceiverService;
        _touchReceiverCanvasService = touchReceiverCanvasService;
        _touchReceiverCanvasService.InitializeCanvas(_maxCanvasSize, TouchCircleSize, s_defaultCanvasWidth);
        _touchReceiverCanvasService.SetUpdateHandler((service) =>
        {
            CanvasWidth = (int)service.CanvasSize.Width;
            CanvasHeight = (int)service.CanvasSize.Height;
            TouchCircleX = (int)service.TouchCirclePosition.X;
            TouchCircleY = (int)service.TouchCirclePosition.Y;
        });

        _serviceOptions = _touchReceiverSettingsService.ServiceOptions;
        _screenOptions = _touchReceiverSettingsService.ScreenOptions;

        var candidateIpAddresses = NetworkHelper.GetAllLocalIPv4();
        IpAddresses = candidateIpAddresses.Count > 0 ? string.Join(" / ", NetworkHelper.GetAllLocalIPv4()) : "Unknown";
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
            // ignore
        }
        StartTouchReceiverServiceCommand.NotifyCanExecuteChanged();
        StopTouchReceiverServiceCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand(CanExecute = nameof(CanStopTouchReceiverService))]
    private void StopTouchReceiverService()
    {
        StartTouchReceiverServiceCommand.Cancel();
        _touchReceiverCanvasService.Stop();
    }
    private bool CanStartTouchReceiverService() => !IsTouchReceiverServiceRunning();
    private bool CanStopTouchReceiverService() => IsTouchReceiverServiceRunning();

    private bool IsTouchReceiverServiceRunning()
    {
        return !_touchReceiverTask?.IsCompleted ?? false;
    }
}
