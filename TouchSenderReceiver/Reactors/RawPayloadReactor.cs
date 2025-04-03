using TouchSenderInterpreter.Models;

using TouchSenderReceiver.Events;
using TouchSenderReceiver.Interfaces;

namespace TouchSenderReceiver.Reactors;

/// <summary>
/// A simple reactor that just forwards the payload to the event.
/// </summary>
public class RawPayloadReactor : ITouchReceiverReactor
{
    public event Action<TouchReceiverEventArgs>? OnReceive;
    public void Receive(TouchSenderPayload payload)
    {
        OnReceive?.Invoke(new TouchReceiverEventArgs { Payload = payload });
    }
}