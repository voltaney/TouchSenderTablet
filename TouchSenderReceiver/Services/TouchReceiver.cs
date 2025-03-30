using System.Net.Sockets;

using TouchSenderInterpreter;

using TouchSenderReceiver.Interfaces;

namespace TouchSenderReceiver.Services;

public class TouchReceiver
{
    protected List<ITouchReceiverReactor> _reactors = [];
    public async Task StartAsync(int portNumber, CancellationToken cancellationToken)
    {
        // UDPでデータを受信
        using (var udp = new UdpClient(portNumber))
        {
            while (true)
            {
                var recvBuffer = await udp.ReceiveAsync(cancellationToken);
                var result = Interpreter.Read(recvBuffer.Buffer);
                if (result.IsSuccess)
                {
                    foreach (var reactor in _reactors)
                    {
                        reactor.Receive(result.Payload!);
                    }
                }
            }
        }
    }

    public void AddReactor(ITouchReceiverReactor reactor)
    {
        _reactors.Add(reactor);
    }

    public void AddReactor<T>(Action<T> configureReactor) where T : ITouchReceiverReactor, new()
    {
        var reactor = new T();
        configureReactor(reactor);
        AddReactor(reactor);
    }
}
