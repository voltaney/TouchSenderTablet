using TouchSenderTablet.GUI.Contracts.Services;
using TouchSenderTablet.GUI.Models;

namespace TouchSenderTablet.GUI.Services;

public class TouchReceiverSettingsService(ILocalSettingsService localSettingsService) : ITouchReceiverSettingsService
{
    private const string ServiceSettingsKey = "TouchReceiverServiceOptions";
    private const string ScreenSettingsKey = "TouchReceiverScreenOptions";
    public TouchReceiverServiceOptions ServiceOptions { get; private set; } = new();
    public TouchReceiverScreenOptions ScreenOptions { get; private set; } = new();

    public async Task InitializeAsync()
    {
        await LoadTouchReceiverOptionsAsync();
        await Task.CompletedTask;
    }

    public async Task LoadTouchReceiverOptionsAsync()
    {
        ServiceOptions = await localSettingsService.ReadSettingAsync<TouchReceiverServiceOptions>(ServiceSettingsKey) ?? ServiceOptions;
        ScreenOptions = await localSettingsService.ReadSettingAsync<TouchReceiverScreenOptions>(ScreenSettingsKey) ?? ScreenOptions;
    }

    public async Task SaveTouchReceiverServiceOptionsAsync(TouchReceiverServiceOptions options)
    {
        await localSettingsService.SaveSettingAsync(ServiceSettingsKey, options);
    }

    public async Task SaveTouchReceiverScreenOptionsAsync(TouchReceiverScreenOptions options)
    {
        await localSettingsService.SaveSettingAsync(ScreenSettingsKey, options);
    }
}
