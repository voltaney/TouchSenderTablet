using TouchSenderTablet.GUI.Contracts.Services;

namespace TouchSenderTablet.GUI.Services;

public class WinUIExPersistenceStorageService(ILocalSettingsService localSettingsService) : IWinUIExPersistenceStorageService
{
    private const string SettingsKey = nameof(WinUIExPersistenceStorageService);

    public IDictionary<string, object>? PersistenceStorage { get; private set; }
    private bool _isInitialized;
    public async Task InitializeAsync()
    {
        // イベントハンドラの追加を含むため、一度きりの初期化を保証
        if (!_isInitialized)
        {
            await LoadFromSettingsAsync();
            App.MainWindow.Closed += async (sender, e) => await SaveToSettingsAsync();
            _isInitialized = true;
        }
    }

    public async Task LoadFromSettingsAsync()
    {
        PersistenceStorage = await localSettingsService.ReadSettingAsync<IDictionary<string, object>>(SettingsKey) ?? new Dictionary<string, object>();
    }

    public async Task SaveToSettingsAsync()
    {
        await localSettingsService.SaveSettingAsync(SettingsKey, PersistenceStorage);
    }
}
