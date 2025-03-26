using TouchSenderInterpreter.Models;

namespace TouchSenderTablet.Core.Interfaces
{
    public interface ITouchSenderReactor
    {
        void OnReceive(TouchSenderPayload payload);
    }
}
