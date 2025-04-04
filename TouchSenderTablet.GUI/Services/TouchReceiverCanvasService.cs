using Microsoft.UI.Xaml;

using TouchSenderInterpreter.Models;

using TouchSenderTablet.GUI.Contracts.Services;

using Windows.Foundation;

namespace TouchSenderTablet.GUI.Services;

/// <summary>
/// TouchSenderアプリの入力状況を定期的に確認し、Canvasのサイズとタッチ位置を計算するサービス
/// </summary>
class TouchReceiverCanvasService : ITouchReceiverCanvasService
{
    private readonly DispatcherTimer _timer;
    private readonly ITouchReceiverService _touchReceiverService;

    private int _maxCanvasSize;
    private int _touchCircleSize;
    private bool _isInitialized = false;
    public Size CanvasSize { get; private set; }
    public Point TouchCirclePosition { get; private set; }
    private Action<Size, Point, int>? _updateHandler;
    private TouchSenderPayload? _currentPayload;

    public TouchReceiverCanvasService(ITouchReceiverService touchReceiverService)
    {
        _touchReceiverService = touchReceiverService;
        _timer = new();
        _timer.Tick += TimerTick;
    }

    public void InitializeCanvas(int maxCanvasSize, int touchCircleSize, int defaultCanvasSize)
    {
        _maxCanvasSize = maxCanvasSize;
        _touchCircleSize = touchCircleSize;
        CanvasSize = new Size(defaultCanvasSize, defaultCanvasSize);
        _isInitialized = true;
    }

    private void TimerTick(object? sender, object e)
    {
        if (_touchReceiverService.TryGetLatestPayload(out _currentPayload))
        {
            CalculateCanvas();
            CalculateTouchCircle();
            _updateHandler?.Invoke(CanvasSize, TouchCirclePosition, _touchReceiverService.DroppedPayloadCount);
        }
    }

    private void CalculateCanvas()
    {
        var device = _currentPayload!.DeviceInfo;
        if (device.Height > device.Width)
        {
            CanvasSize = new Size((double)device.Width / device.Height * _maxCanvasSize, _maxCanvasSize);
        }
        else
        {
            CanvasSize = new Size(_maxCanvasSize, (double)device.Height / device.Width * _maxCanvasSize);
        }
    }

    private void CalculateTouchCircle()
    {
        if (_currentPayload!.SingleTouch is null)
        {
            // タッチ位置が存在しない場合は画面外に配置
            TouchCirclePosition = new Point(-_touchCircleSize, -_touchCircleSize);
            return;
        }
        var ratioX = _currentPayload.SingleTouch.X / _currentPayload.DeviceInfo.Width;
        var ratioY = _currentPayload.SingleTouch.Y / _currentPayload.DeviceInfo.Height;
        TouchCirclePosition = new Point(ratioX * CanvasSize.Width - _touchCircleSize / 2.0, ratioY * CanvasSize.Height - _touchCircleSize / 2.0);
    }

    public void SetUpdateHandler(Action<Size, Point, int> updateHandler)
    {
        _updateHandler = updateHandler;
    }

    public void Start(TimeSpan timeSpan)
    {
        if (_timer.IsEnabled)
        {
            Stop();
        }
        if (!_isInitialized)
        {
            throw new InvalidOperationException("Canvas is not initialized.");
        }
        _timer.Interval = timeSpan;
        _timer.Start();
    }

    public void Stop()
    {
        _timer.Stop();
    }
}
