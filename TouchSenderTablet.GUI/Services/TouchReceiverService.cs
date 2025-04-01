using Microsoft.Extensions.Logging;

using TouchSenderInterpreter.Models;

using TouchSenderReceiver.Reactors;
using TouchSenderReceiver.Services;

using TouchSenderTablet.GUI.Contracts.Services;
using TouchSenderTablet.GUI.Models;

using WindowsInput;

namespace TouchSenderTablet.GUI.Services;

public class TouchReceiverService(ILogger<TouchReceiverService> logger) : ITouchReceiverService
{
    private TouchReceiver? _receiver;
    public TouchSenderPayload? CurrentPayload { get; private set; }
    public bool IsDataReceived => CurrentPayload is not null;
    private int _portNumber;
    static readonly InputSimulator inputSimulator = new InputSimulator();

    public void SetOptions(TouchReceiverServiceOptions options)
    {
        _portNumber = options.PortNumber;
        _receiver = new TouchReceiver();
        _receiver.AddReactor<SingleTouchReactor>((r) =>
        {
            r.OnWhileTouched += (e) =>
            {
                if (e.Offset is null) return;
                if (options.LeftClickWhileTouched)
                {
                    inputSimulator.Mouse.LeftButtonDown();
                }
                inputSimulator.Mouse.MoveMouseBy(
                    (int)Math.Round(e.Offset.X * (options.HorizontalSensitivity) / 38.0),
                    (int)Math.Round(e.Offset.Y * (options.VerticalSensitivity) / 38.0));
            };
            r.OnReleased += (e) =>
            {
                if (options.LeftClickWhileTouched)
                {
                    inputSimulator.Mouse.LeftButtonUp();
                }
            };
        });
        // TODO: 1000hzくらいのときに外部からデータを取れない。
        // スレッドセーフでないため。
        _receiver.AddReactor<RawPayloadReactor>((r) =>
        {
            r.OnReceive += (e) =>
            {
                CurrentPayload = e.Payload;
            };
        });
    }

    public async Task StartAsync(CancellationToken token)
    {
        if (_receiver != null)
        {
            logger.LogInformation("TouchReceiverService is starting");
            CurrentPayload = null;
            await _receiver.StartAsync(_portNumber, token);
        }
    }
}
