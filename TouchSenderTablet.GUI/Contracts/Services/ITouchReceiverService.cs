using TouchSenderInterpreter.Models;

using TouchSenderTablet.GUI.Models;

namespace TouchSenderTablet.GUI.Contracts.Services;

public interface ITouchReceiverService
{
    TouchSenderPayload? CurrentPayload { get; }
    void SetOptions(TouchReceiverServiceOptions options);
    Task StartAsync(CancellationToken token);
}
