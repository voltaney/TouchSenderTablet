using TouchSenderTablet.Core.Events;
using TouchSenderTablet.Core.Implementations;
using TouchSenderTablet.Core.Services;

namespace TouchSenderTablet.Core.Extensions
{
    public static class TouchSenderReceiverExtensions
    {
        public static void AddFirstTimeOnlyReactor(this TouchSenderReceiver receiver, Action<TouchSenderEventArgs> onFirstReceive)
        {
            var reactor = new FirstTimeOnlyReactor();
            reactor.OnFirstReceive += onFirstReceive;
            receiver.AddReactor(reactor);
        }
    }
}
