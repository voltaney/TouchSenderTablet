using TouchSenderInterpreter.Models;

using TouchSenderTablet.Core.Events;
using TouchSenderTablet.Core.Interfaces;

namespace TouchSenderTablet.Core.Implementations
{
    public class FirstTimeOnlyReactor : ITouchSenderReactor
    {
        private bool _hasReceived = false;
        public event Action<TouchSenderEventArgs>? OnFirstReceive;
        public void OnReceive(TouchSenderPayload payload)
        {
            if (!_hasReceived)
            {
                _hasReceived = true;

                OnFirstReceive?.Invoke(new TouchSenderEventArgs { Payload = payload });
            }
        }
    }
}
