using TouchSenderInterpreter.Models;

using TouchSenderReceiver.Events;
using TouchSenderReceiver.Interfaces;

namespace TouchSenderReceiver.Implementations
{
    public class FirstTimeOnlyReactor : ITouchSenderReactor
    {
        private bool _hasReceived = false;
        public event Action<TouchReceiverEventArgs>? OnFirstReceive;
        public void Receive(TouchSenderPayload payload)
        {
            if (!_hasReceived)
            {
                _hasReceived = true;

                OnFirstReceive?.Invoke(new TouchReceiverEventArgs { Payload = payload });
            }
        }
    }
}
