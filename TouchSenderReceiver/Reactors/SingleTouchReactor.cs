using TouchSenderInterpreter.Models;

using TouchSenderReceiver.Events;
using TouchSenderReceiver.Interfaces;

namespace TouchSenderReceiver.Reactors;

public class SingleTouchReactor : ITouchReceiverReactor
{
    private SingleTouch? _previous;
    public event Action<SingleTouchEventArgs>? OnWhileTouched;
    public event Action<SingleTouchEventArgs>? OnWhileReleased;
    public event Action<SingleTouchEventArgs>? OnTouched;
    public event Action<SingleTouchEventArgs>? OnReleased;

    public void Receive(TouchSenderPayload payload)
    {
        InvokeActions(payload);
        SavePreviousPayload(payload);
    }

    private void InvokeActions(TouchSenderPayload payload)
    {
        var args = new SingleTouchEventArgs
        {
            DeviceWidth = payload.DeviceInfo.Width,
            DeviceHeight = payload.DeviceInfo.Height,
            Current = payload.SingleTouch,
            Previous = _previous,
        };
        if (payload.SingleTouch is not null)
        {
            OnWhileTouched?.Invoke(args);
            if (_previous is null)
            {
                OnTouched?.Invoke(args);
            }
        }
        else
        {
            OnWhileReleased?.Invoke(args);
            if (_previous is not null)
            {
                OnReleased?.Invoke(args);
            }
        }
    }

    private void SavePreviousPayload(TouchSenderPayload payload)
    {
        _previous = payload.SingleTouch;
    }
}
