﻿using Microsoft.UI.Xaml.Controls;

using TouchSenderTablet.GUI.ViewModels;

namespace TouchSenderTablet.GUI.Views;

public sealed partial class MainPage : Page
{
    public MainViewModel ViewModel
    {
        get;
    }

    public MainPage()
    {
        ViewModel = App.GetService<MainViewModel>();
        InitializeComponent();
    }

    private async void Page_Unloaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        await ViewModel.SaveOptions();
    }
}
