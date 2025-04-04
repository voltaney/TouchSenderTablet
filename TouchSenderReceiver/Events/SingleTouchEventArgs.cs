using TouchSenderInterpreter.Models;

namespace TouchSenderReceiver.Events;

public class SingleTouchEventArgs
{
    public int DeviceWidth { get; init; }
    public int DeviceHeight { get; init; }
    public required SingleTouch? Previous { get; init; }
    public required SingleTouch? Current { get; init; }

    public SingleTouch? Offset
    {
        get
        {
            if (Previous is null || Current is null)
            {
                return null;
            }
            return new(
                X: Current.X - Previous.X,
                Y: Current.Y - Previous.Y
            );
        }
    }
}
