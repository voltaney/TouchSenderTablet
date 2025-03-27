using System.Net.Sockets;

using TouchSenderInterpreter;

using TouchSenderReceiver.Interfaces;

namespace TouchSenderReceiver.Services
{
    public class TouchReceiver
    {
        protected List<ITouchSenderReactor> _reactors = [];
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

        public void AddReactor(ITouchSenderReactor reactor)
        {
            _reactors.Add(reactor);
        }
    }
}
