using System.Net.Sockets;

using TouchSenderInterpreter;

using TouchSenderTablet.Core.Interfaces;

namespace TouchSenderTablet.Core.Services
{
    public class TouchSenderReceiver
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
                            reactor.OnReceive(result.Payload!);
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
