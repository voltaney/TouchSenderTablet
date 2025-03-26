using TouchSenderTablet.Core.Interfaces;
using TouchSenderTablet.Core.Services;

namespace TouchSenderTablet.Core.Extensions
{
    public static class TouchSenderReceiverExtensions
    {
        public static void AddReactor<T>(this TouchSenderReceiver receiver,
            Action<T> configureReactor) where T : ITouchSenderReactor, new()
        {
            var reactor = new T();
            configureReactor(reactor);
            receiver.AddReactor(reactor);
        }
    }
}
