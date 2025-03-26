using TouchSenderInterpreter.Models;

namespace TouchSenderTablet.Core.Interfaces
{
    public interface ITouchSenderReactor
    {
        void Receive(TouchSenderPayload payload);
    }
}
