using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using TouchSenderTablet.GUI.Helpers;
using TouchSenderTablet.GUI.ViewModels;

namespace TouchSenderTablet.GUI.Views;

// TODO: Set the URL for your privacy policy by updating SettingsPage_PrivacyTermsLink.NavigateUri in Resources.resw.
public sealed partial class SettingsPage : Page
{
    public SettingsViewModel ViewModel
    {
        get;
    }

    public SettingsPage()
    {
        ViewModel = App.GetService<SettingsViewModel>();
        InitializeComponent();
    }

    private void OnThemeSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedTheme = ((ComboBoxItem)ThemeMode.SelectedItem)?.Tag?.ToString();
        if (selectedTheme != null)
        {
            ViewModel.SwitchThemeCommand.Execute(EnumHelper.GetEnum<ElementTheme>(selectedTheme));
        }
    }
}
