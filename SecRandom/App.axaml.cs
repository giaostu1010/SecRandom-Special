using System;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using SecRandom.Core.Abstraction;
using SecRandom.Core.Extensions.Registry;
using SecRandom.Core.Services.Logging;
using SecRandom.Services.Config;
using SecRandom.ViewModels;
using SecRandom.Views;
using SecRandom.Views.MainPages;
using SecRandom.Views.SettingsPages;

namespace SecRandom;

public partial class App : Application
{
    public static FloatingWindow? FloatingWindow;
    public static MainWindow? MainWindow;
    public static MainWindow? SettingsWindow;
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        InitializeLanguages(new CultureInfo("zh-hans"));
        BuildHost();
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();

            FloatingWindow = new FloatingWindow();
            desktop.MainWindow = FloatingWindow;
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime)
        {
            throw new PlatformNotSupportedException();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }

    private void InitializeLanguages(CultureInfo cultureInfo)
    {
        Langs.Resources.Culture = cultureInfo;
        Langs.RollCallPage.Resources.Culture = cultureInfo;
    }
    
    private void BuildHost()
    {
        IAppHost.Host = Host
            .CreateDefaultBuilder()
            .UseContentRoot(AppContext.BaseDirectory)
            .ConfigureServices(services =>
            {
                // 日志
                services.AddLogging(builder =>
                {
                    builder.AddConsoleFormatter<ClassIslandConsoleFormatter, ConsoleFormatterOptions>();
                    builder.AddConsole(console => { console.FormatterName = "classisland"; });
#if DEBUG
                    builder.SetMinimumLevel(LogLevel.Trace);
#endif
                });
                
                // 配置
                services.AddSingleton<ConfigServiceBase, DesktopConfigService>();
                services.AddSingleton<MainConfigHandler>();
                
                // 服务
                
                // 窗口
                services.AddTransient<MainView>();
                services.AddTransient<MainViewModel>();
                
                services.AddTransient<SettingsView>();
                services.AddTransient<SettingsViewModel>();
                
                // 界面 Views
                services.AddMainPage<RollCallPage>(Langs.Resources.RollCall);
                
                services.AddSettingsPage<AboutPage>(Langs.Resources.About);

                // 界面 ViewModels
            })
            .Build();

        var logger = IAppHost.GetService<ILogger<App>>();
        logger.LogInformation("SecRandom  Copyright by SECTL(2025~{YEAR})  Licensed under GPL3.0", DateTime.Now.Year);
        logger.LogInformation("Host built.");
        
        IAppHost.GetService<MainConfigHandler>();
    }

    public static void ShowMainWindow()
    {
        if (MainWindow is not { IsLoaded: true })
        {
            MainWindow = new MainWindow
            {
                Content = IAppHost.GetService<MainView>(),
                Title = "SecRandom"
            };
        }

        MainWindow.Show();
        MainWindow.Activate();
    }
    
    public static void ShowSettingsWindow()
    {
        if (SettingsWindow is not { IsLoaded: true })
        {
            SettingsWindow = new MainWindow
            {
                Content = IAppHost.GetService<SettingsView>(),
                Title = "SecRandom"
            };
        }

        SettingsWindow.Show();
        SettingsWindow.Activate();
    }
}