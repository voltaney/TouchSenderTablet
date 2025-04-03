using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;

using NLog.Extensions.Logging;

using TouchSenderTablet.GUI.Activation;
using TouchSenderTablet.GUI.Contracts.Services;
using TouchSenderTablet.GUI.Core.Contracts.Services;
using TouchSenderTablet.GUI.Core.Services;
using TouchSenderTablet.GUI.Models;
using TouchSenderTablet.GUI.Services;
using TouchSenderTablet.GUI.ViewModels;
using TouchSenderTablet.GUI.Views;

namespace TouchSenderTablet.GUI;

// To learn more about WinUI 3, see https://docs.microsoft.com/windows/apps/winui/winui3/.
public partial class App : Application
{
    // The .NET Generic Host provides dependency injection, configuration, logging, and other services.
    // https://docs.microsoft.com/dotnet/core/extensions/generic-host
    // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
    // https://docs.microsoft.com/dotnet/core/extensions/configuration
    // https://docs.microsoft.com/dotnet/core/extensions/logging
    public IHost Host
    {
        get;
    }

    public static T GetService<T>()
        where T : class
    {
        if ((App.Current as App)!.Host.Services.GetService(typeof(T)) is not T service)
        {
            throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
        }

        return service;
    }

    public static WindowEx MainWindow { get; } = new MainWindow();

    public static UIElement? AppTitlebar { get; set; }

    // Appクラス内でのみ使用するロガー（UnhandledException関連）
    private readonly ILogger _logger;

    public App()
    {
        InitializeComponent();

        Host = Microsoft.Extensions.Hosting.Host.
        CreateDefaultBuilder().
        UseContentRoot(AppContext.BaseDirectory).
        ConfigureServices((context, services) =>
        {
            // Default Activation Handler
            services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

            // Other Activation Handlers

            // Services
            services.AddSingleton<ILocalSettingsService, LocalSettingsService>();
            services.AddSingleton<ITouchReceiverSettingsService, TouchReceiverSettingsService>();
            services.AddSingleton<ITouchReceiverCanvasService, TouchReceiverCanvasService>();
            services.AddSingleton<ITouchReceiverService, TouchReceiverService>();
            services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
            services.AddTransient<INavigationViewService, NavigationViewService>();

            services.AddSingleton<IActivationService, ActivationService>();
            services.AddSingleton<IPageService, PageService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IWinUIExPersistenceStorageService, WinUIExPersistenceStorageService>();

            // Core Services
            services.AddSingleton<IFileService, FileService>();

            // Views and ViewModels
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<SettingsPage>();
            services.AddTransient<MainViewModel>();
            services.AddTransient<MainPage>();
            services.AddTransient<ShellPage>();
            services.AddTransient<ShellViewModel>();

            // Configuration
            services.Configure<LocalSettingsOptions>(context.Configuration.GetSection(nameof(LocalSettingsOptions)));
        }).
        ConfigureLogging((context, builder) =>
        {
            builder.ClearProviders();
            builder.SetMinimumLevel(LogLevel.Trace);
            builder.AddNLog(context.Configuration);
        }).
        Build();

        // Unhandled exception用にロガーを取得
        _logger = GetService<ILogger<App>>();

        UnhandledException += App_UnhandledException;
        TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
    }

    /// <summary>
    /// アプリケーションで処理されていない例外が発生した場合に呼び出されます。
    /// NOTE: こちらはUIスレッドの処理されていない例外なんかをキャッチできます。
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        // アプリのクラッシュを防ぐ
        e.Handled = true;

        _logger.LogError(e.Exception, "Unhandled exception occurred");

        // トースト通知作成
        var notification = new AppNotificationBuilder()
            .AddText("An exception was thrown.")
            .AddText($"Type: {e.Exception.GetType()}")
            .AddText($"Message: {e.Message}\r\n")
            .BuildNotification();

        // トースト通知を表示
        AppNotificationManager.Default.Show(notification);
        // 1ミリ秒待機することで、アプリケーションが終了する前にトースト通知を表示
        // https://github.com/microsoft/WindowsAppSDK/issues/3437
        Thread.Sleep(1);
        // アプリケーションを終了します
        Environment.Exit(1);
    }

    /// <summary>
    /// タスクスケジューラで処理されていない例外が発生した場合に呼び出されます。
    /// NOTE: まだ一度もこちらのUnhandledExceptionイベントを確認できておらず...
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        _logger.LogError(e.Exception, "Unobserved task exception occurred");

        // アプリケーションを終了します
        e.SetObserved();
    }

    /// <summary>
    /// アプリケーションドメインで処理されない例外が発生した場合に呼び出されます。
    /// NOTE: まだ一度もこちらのUnhandledExceptionイベントを確認できておらず...
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CurrentDomain_UnhandledException(object sender, System.UnhandledExceptionEventArgs e)
    {
        var exception = e.ExceptionObject as Exception;

        _logger.LogError(exception, "AppDomain.CurrentDomain unhandled exception occurred");

        // アプリケーションを終了します
        Environment.Exit(1);
    }

    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);

        await App.GetService<IActivationService>().ActivateAsync(args);
    }
}
