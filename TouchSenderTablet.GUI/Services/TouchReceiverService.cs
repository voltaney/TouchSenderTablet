using TouchSenderInterpreter.Models;

using TouchSenderReceiver.Reactors;
using TouchSenderReceiver.Services;

using TouchSenderTablet.GUI.Contracts.Services;
using TouchSenderTablet.GUI.Models;

using WindowsInput;

namespace TouchSenderTablet.GUI.Services;

public class TouchReceiverService : ITouchReceiverService
{
    private TouchReceiver? _receiver;
    public TouchSenderPayload? CurrentPayload { get; private set; }
    private int _portNumber;

    public void SetOptions(TouchReceiverServiceOptions options)
    {
        _portNumber = options.PortNumber;
        var inputSimulator = new InputSimulator();
        _receiver = new TouchReceiver();
        _receiver.AddReactor<SingleTouchReactor>((r) =>
        {
            r.OnWhileTouched += (e) =>
            {
                if (e.LongSideOffsetRatio is null) return;
                //inputSimulator.Mouse.LeftButtonDown();
                inputSimulator.Mouse.MoveMouseBy(
                    (int)(e.LongSideOffsetRatio.X * options.HorizontalSensitivity),
                    (int)(e.LongSideOffsetRatio.Y * options.VerticalSensitivity));
            };
        });
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
            await _receiver.StartAsync(_portNumber, token);
        }
    }
}
