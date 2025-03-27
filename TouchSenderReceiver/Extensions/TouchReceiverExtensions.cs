using TouchSenderReceiver.Interfaces;
using TouchSenderReceiver.Services;

namespace TouchSenderReceiver.Extensions
{
    public static class TouchReceiverExtensions
    {
        public static void AddReactor<T>(this TouchReceiver receiver,
            Action<T> configureReactor) where T : ITouchSenderReactor, new()
        {
            var reactor = new T();
            configureReactor(reactor);
            receiver.AddReactor(reactor);
        }
    }
}
