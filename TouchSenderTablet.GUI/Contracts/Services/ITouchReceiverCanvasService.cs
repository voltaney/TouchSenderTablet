using Windows.Foundation;

namespace TouchSenderTablet.GUI.Services;

public interface ITouchReceiverCanvasService
{
    Size CanvasSize { get; }
    Point TouchCirclePosition { get; }

    void InitializeCanvas(int maxCanvasSize, int touchCircleSize, int defaultCanvasSize);
    void SetUpdateHandler(Action<ITouchReceiverCanvasService> updateHandler);
    void Start(TimeSpan timeSpan);
    void Stop();
}