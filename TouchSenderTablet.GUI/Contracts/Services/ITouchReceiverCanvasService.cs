using Windows.Foundation;

namespace TouchSenderTablet.GUI.Contracts.Services;

public interface ITouchReceiverCanvasService
{
    Size CanvasSize { get; }
    Point TouchCirclePosition { get; }

    void InitializeCanvas(int maxCanvasSize, int touchCircleSize, int defaultCanvasSize);
    void SetUpdateHandler(Action<Size, Point, int> updateHandler);
    void Start(TimeSpan timeSpan);
    void Stop();
}
