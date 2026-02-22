using System;
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
    public static MainWindow? MainWindow;
    public static MainWindow? SettingsWindow;
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        BuildHost();
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();

            MainWindow = new MainWindow
            {
                Content = IAppHost.GetService<MainView>(),
                Title="SecRandom"
            };
            desktop.MainWindow = MainWindow;
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
                services.AddSingleton<MainView>();
                services.AddTransient<MainViewModel>();
                
                services.AddTransient<SettingsView>();
                services.AddTransient<SettingsViewModel>();
                
                // 界面 Views
                services.AddMainPage<RollCallPage>();
                
                services.AddSettingsPage<AboutPage>();

                // 界面 ViewModels
            })
            .Build();

        var logger = IAppHost.GetService<ILogger<App>>();
        logger.LogInformation("SecRandom  Copyright by SECTL(2025~{YEAR})  Licensed under GPL3.0", DateTime.Now.Year);
        logger.LogInformation("Host built.");
        
        IAppHost.GetService<MainConfigHandler>();
    }

    public static void ShowSettingsWindow()
    {
        if (SettingsWindow is { IsVisible: true })
        {
            SettingsWindow.Activate();
            return;
        }
        
        if (SettingsWindow is not { IsLoaded: true })
        {
            SettingsWindow = new MainWindow
            {
                Content = IAppHost.GetService<SettingsView>(),
                Title = "SecRandom"
            };
            SettingsWindow.Closed += (_, _) => SettingsWindow = null;
        }

        SettingsWindow.Show();
        SettingsWindow.Activate();
    }
}
