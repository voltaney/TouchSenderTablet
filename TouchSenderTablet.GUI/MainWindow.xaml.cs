using CommunityToolkit.Mvvm.Messaging;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using TouchSenderTablet.GUI.Helpers;
using TouchSenderTablet.GUI.Models;
using TouchSenderTablet.GUI.Views;

using Windows.UI.ViewManagement;

namespace TouchSenderTablet.GUI;

public sealed partial class MainWindow : WindowEx
{
    private readonly Microsoft.UI.Dispatching.DispatcherQueue _dispatcherQueue;

    private readonly UISettings _settings;

    public MainWindow()
    {
        InitializeComponent();

        AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets/WindowIcon.ico"));
        Content = null;
        Title = "AppDisplayName".GetLocalized();

        // Theme change code picked from https://github.com/microsoft/WinUI-Gallery/pull/1239
        _dispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();
        _settings = new UISettings();
        _settings.ColorValuesChanged += Settings_ColorValuesChanged; // cannot use FrameworkElement.ActualThemeChanged event

        WeakReferenceMessenger.Default.Register<ShowErrorDialogMessage>(this, ShowErrorDialogAsync);
    }

    // this handles updating the caption button colors correctly when indows system theme is changed
    // while the app is open
    private void Settings_ColorValuesChanged(UISettings sender, object args)
    {
        // This calls comes off-thread, hence we will need to dispatch it to current app's thread
        _dispatcherQueue.TryEnqueue(() =>
        {
            TitleBarHelper.ApplySystemThemeToCaptionButtons();
        });
    }

    private async void ShowErrorDialogAsync(object recipient, ShowErrorDialogMessage message)
    {
        var dialog = new ContentDialog
        {
            XamlRoot = Content.XamlRoot,
            Title = message.Title,
            CloseButtonText = "Close",
            RequestedTheme = ((FrameworkElement)Content).ActualTheme,
            Content = new ErrorDialogContent { ErrorMessage = message.Message ?? "", ErrorDetail = message.Error?.Message ?? "" }
        };

        await dialog.ShowAsync();
    }
}
