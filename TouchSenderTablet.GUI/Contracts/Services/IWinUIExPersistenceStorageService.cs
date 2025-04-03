namespace TouchSenderTablet.GUI.Contracts.Services;

public interface IWinUIExPersistenceStorageService
{
    IDictionary<string, object>? PersistenceStorage { get; }

    Task InitializeAsync();
    Task LoadFromSettingsAsync();
    Task SaveToSettingsAsync();
}
