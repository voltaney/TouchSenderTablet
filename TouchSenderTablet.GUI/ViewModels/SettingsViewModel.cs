using System.Diagnostics;
using System.Reflection;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Microsoft.UI.Xaml;

using TouchSenderTablet.GUI.Contracts.Services;
using TouchSenderTablet.GUI.Helpers;
using TouchSenderTablet.GUI.Models;

using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.Search;

namespace TouchSenderTablet.GUI.ViewModels;

public partial class SettingsViewModel : ObservableRecipient
{
    private readonly IThemeSelectorService _themeSelectorService;

    [ObservableProperty]
    public partial ElementTheme ElementTheme { get; set; }

    [ObservableProperty]
    public partial string VersionDescription { get; set; }

    [ObservableProperty]
    public partial string Version { get; set; }

    [ObservableProperty]
    public partial string AppDisplayName { get; set; }

    public ICommand SwitchThemeCommand
    {
        get;
    }

    public SettingsViewModel(IThemeSelectorService themeSelectorService)
    {
        _themeSelectorService = themeSelectorService;
        ElementTheme = _themeSelectorService.Theme;
        VersionDescription = GetVersionDescription();
        Version = GetVersion();
        AppDisplayName = "AppDisplayName".GetLocalized();

        SwitchThemeCommand = new RelayCommand<ElementTheme>(
            async (param) =>
            {
                if (ElementTheme != param)
                {
                    ElementTheme = param;
                    await _themeSelectorService.SetThemeAsync(param);
                }
            });
    }

    private static string GetVersion()
    {
        Version version;

        if (RuntimeHelper.IsMSIX)
        {
            var packageVersion = Package.Current.Id.Version;

            version = new(packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision);
        }
        else
        {
            version = Assembly.GetExecutingAssembly().GetName().Version!;
        }
        return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
    }

    private static string GetVersionDescription()
    {
        Version version;

        if (RuntimeHelper.IsMSIX)
        {
            var packageVersion = Package.Current.Id.Version;

            version = new(packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision);
        }
        else
        {
            version = Assembly.GetExecutingAssembly().GetName().Version!;
        }

        return $"{"AppDisplayName".GetLocalized()} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
    }

    [RelayCommand]
    private async Task ShowLogFile(CancellationToken token)
    {
        var queryOpitons = new QueryOptions(CommonFileQuery.OrderByName, [".log"]);
        var result = ApplicationData.Current.LocalCacheFolder.CreateFileQueryWithOptions(queryOpitons);

        var logFile = (await result.GetFilesAsync()).Where(f => f.Name.Equals("app.log")).FirstOrDefault();

        if (logFile is null)
        {
            WeakReferenceMessenger.Default.Send(new ShowErrorDialogMessage()
            {
                Title = "File Not Found",
                Message = "No log files have been generated yet.",
            });
        }
        else
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "explorer",
                Arguments = $"/select,\"{logFile.Path}\"",
                UseShellExecute = true
            });
        }
    }
}
