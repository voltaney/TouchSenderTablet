using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using TouchSenderTablet.GUI.Activation;
using TouchSenderTablet.GUI.Contracts.Services;
using TouchSenderTablet.GUI.Helpers;
using TouchSenderTablet.GUI.Views;

namespace TouchSenderTablet.GUI.Services;

public class ActivationService(
    ActivationHandler<LaunchActivatedEventArgs> defaultHandler,
    IEnumerable<IActivationHandler> activationHandlers,
    IThemeSelectorService themeSelectorService,
    ITouchReceiverSettingsService touchReceiverSettingsService,
    IWinUIExPersistenceStorageService winUIExPersistenceStorageService) : IActivationService
{
    private UIElement? _shell = null;

    public async Task ActivateAsync(object activationArgs)
    {
        // Execute tasks before activation.
        await InitializeAsync();

        // Set the MainWindow Content.
        if (App.MainWindow.Content == null)
        {
            _shell = App.GetService<ShellPage>();
            App.MainWindow.Content = _shell ?? new Frame();
        }
        // Set WinUIEx PersistenceStorage if running as unpackaged app.
        if (!RuntimeHelper.IsMSIX)
        {
            await winUIExPersistenceStorageService.InitializeAsync();
            WindowManager.PersistenceStorage = winUIExPersistenceStorageService.PersistenceStorage;
        }

        // Handle activation via ActivationHandlers.
        await HandleActivationAsync(activationArgs);

        // Activate the MainWindow.
        App.MainWindow.Activate();

        // Execute tasks after activation.
        await StartupAsync();
    }

    private async Task HandleActivationAsync(object activationArgs)
    {
        var activationHandler = activationHandlers.FirstOrDefault(h => h.CanHandle(activationArgs));

        if (activationHandler != null)
        {
            await activationHandler.HandleAsync(activationArgs);
        }

        if (defaultHandler.CanHandle(activationArgs))
        {
            await defaultHandler.HandleAsync(activationArgs);
        }
    }

    private async Task InitializeAsync()
    {
        await touchReceiverSettingsService.InitializeAsync();
        await themeSelectorService.InitializeAsync().ConfigureAwait(false);
        await Task.CompletedTask;
    }

    private async Task StartupAsync()
    {
        await themeSelectorService.SetRequestedThemeAsync();
        await Task.CompletedTask;
    }
}
