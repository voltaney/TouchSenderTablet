using TouchSenderInterpreter.Models;

namespace TouchSenderReceiver.Events
{
    public class TouchReceiverEventArgs
    {
        public required TouchSenderPayload Payload { get; init; }
    }
}
