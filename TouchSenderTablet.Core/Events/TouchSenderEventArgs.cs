using TouchSenderInterpreter.Models;

namespace TouchSenderTablet.Core.Events
{
    public class TouchSenderEventArgs
    {
        public required TouchSenderPayload Payload { get; init; }
    }
}
