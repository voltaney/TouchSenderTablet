using TouchSenderInterpreter.Models;

using TouchSenderTablet.GUI.Models;

namespace TouchSenderTablet.GUI.Contracts.Services;

public interface ITouchReceiverService
{
    int DroppedPayloadCount { get; }
    void SetOptions(TouchReceiverServiceOptions options);
    Task StartAsync(CancellationToken token);
    bool TryGetLatestPayload(out TouchSenderPayload? payload);
}
