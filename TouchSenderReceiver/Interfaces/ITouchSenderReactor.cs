using TouchSenderInterpreter.Models;

namespace TouchSenderReceiver.Interfaces
{
    public interface ITouchSenderReactor
    {
        void Receive(TouchSenderPayload payload);
    }
}
