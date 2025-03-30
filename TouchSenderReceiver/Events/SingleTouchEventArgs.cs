using TouchSenderInterpreter.Models;

namespace TouchSenderReceiver.Events;

public class SingleTouchEventArgs
{
    public int DeviceWidth { get; init; }
    public int DeviceHeight { get; init; }
    public required SingleTouch? Previous { get; init; }
    public required SingleTouch? Current { get; init; }
    public required SingleTouch? PreviousRatio { get; init; }
    public required SingleTouch? CurrentRatio { get; init; }
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

    public SingleTouch? OffsetRatio
    {
        get
        {
            if (PreviousRatio is null || CurrentRatio is null)
            {
                return null;
            }
            return new(
                X: CurrentRatio.X - PreviousRatio.X,
                Y: CurrentRatio.Y - PreviousRatio.Y
            );
        }
    }

    public SingleTouch? LongSideOffsetRatio
    {
        get
        {
            if (Previous is null || Current is null)
            {
                return null;
            }
            return new(
                X: (Current.X - Previous.X) / Math.Max(DeviceHeight, DeviceWidth),
                Y: (Current.Y - Previous.Y) / Math.Max(DeviceHeight, DeviceWidth)
            );
        }
    }
}
