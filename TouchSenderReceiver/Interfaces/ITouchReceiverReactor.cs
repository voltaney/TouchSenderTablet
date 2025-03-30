using TouchSenderInterpreter.Models;

namespace TouchSenderReceiver.Interfaces;

public interface ITouchReceiverReactor
{
    void Receive(TouchSenderPayload payload);
}
