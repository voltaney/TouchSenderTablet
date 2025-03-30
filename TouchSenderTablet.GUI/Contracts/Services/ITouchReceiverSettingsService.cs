using TouchSenderTablet.GUI.Models;

namespace TouchSenderTablet.GUI.Contracts.Services;

public interface ITouchReceiverSettingsService
{
    TouchReceiverServiceOptions ServiceOptions { get; }
    TouchReceiverScreenOptions ScreenOptions { get; }

    Task InitializeAsync();
    Task LoadTouchReceiverOptionsAsync();
    Task SaveTouchReceiverServiceOptionsAsync(TouchReceiverServiceOptions options);
    Task SaveTouchReceiverScreenOptionsAsync(TouchReceiverScreenOptions options);
}
