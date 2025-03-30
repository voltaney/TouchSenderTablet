
using Microsoft.UI.Xaml;

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
    private Action<ITouchReceiverCanvasService>? _updateHandler;

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
        CalculateCanvas();
        CalculateTouchCircle();
        _updateHandler?.Invoke(this);
    }

    private void CalculateCanvas()
    {
        if (_touchReceiverService.CurrentPayload is null)
        {
            return;
        }
        var device = _touchReceiverService.CurrentPayload.DeviceInfo;
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
        if (_touchReceiverService.CurrentPayload?.SingleTouchRatio is null)
        {
            return;
        }
        var ratio = _touchReceiverService.CurrentPayload.SingleTouchRatio;
        TouchCirclePosition = new Point(ratio.X * CanvasSize.Width - _touchCircleSize / 2.0, ratio.Y * CanvasSize.Height - _touchCircleSize / 2.0);
    }

    public void SetUpdateHandler(Action<ITouchReceiverCanvasService> updateHandler)
    {
        _updateHandler = updateHandler;
    }

    public void Start(TimeSpan timeSpan)
    {
        if (_timer.IsEnabled)
        {
            throw new InvalidOperationException("Timer is already running.");
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

    public void SetUpdateHandler(Action updateHandler)
    {
        throw new NotImplementedException();
    }

    public void InitializeCanvas(int maxCanvasSize, int touchCircleSize)
    {
        throw new NotImplementedException();
    }
}
