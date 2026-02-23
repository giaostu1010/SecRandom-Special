using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Styling;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using SecRandom.Core;
using SecRandom.Core.Abstraction;
using SecRandom.Core.Extensions.Registry;
using SecRandom.Core.Enums;
using SecRandom.Core.Services;
using SecRandom.Core.Services.Logging;
using SecRandom.Services.Config;
using SecRandom.ViewModels;
using SecRandom.Views;
using SecRandom.Views.MainPages;
using SecRandom.Views.SettingsPages;

namespace SecRandom;

public partial class App : Application
{
    private static MainWindow? _mainWindow;
    private static MainWindow? _settingsWindow;
    private static CultureInfo? _startupCulture;
    private static UiLanguageMode _startupUiLanguageMode;
    private static string? _startupConfigFilePath;
    private static readonly string[] _legacyUiFontFamilies = ["HarmonyOS Sans SC", "Segoe UI", "Microsoft YaHei UI"];
    private static readonly FontWeight[] _uiFontWeights = [FontWeight.Normal, FontWeight.SemiBold, FontWeight.Bold];
    private static readonly Dictionary<string, Func<string>> _pageNameProviders = new()
    {
        ["main.rollCall"] = () => Langs.Common.Resources.RollCall,
        ["settings.basic"] = () => Langs.Common.Resources.BasicSettings,
        ["settings.rosterManagement"] = () => Langs.Common.Resources.RosterManagement,
        ["settings.draw.rollCall"] = () => Langs.SettingsPages.DrawSettingsPage.Resources.RollCallSettings,
        ["settings.draw.quickDraw"] = () => Langs.SettingsPages.DrawSettingsPage.Resources.QuickDrawSettings,
        ["settings.draw.lottery"] = () => Langs.SettingsPages.DrawSettingsPage.Resources.LotterySettings,
        ["settings.draw.faceDetector"] = () => Langs.SettingsPages.DrawSettingsPage.Resources.FaceDetectorSettings,
        ["settings.floatingWindow"] = () => Langs.Common.Resources.FloatingWindowManagement,
        ["settings.notificationSettings"] = () => Langs.Common.Resources.NotificationSettings,
        ["settings.securitySettings"] = () => Langs.Common.Resources.SecuritySettings,
        ["settings.linkageSettings"] = () => Langs.Common.Resources.LinkageSettings,
        ["settings.voiceSettings"] = () => Langs.Common.Resources.VoiceSettings,
        ["settings.themeManagement"] = () => Langs.Common.Resources.ThemeManagement,
        ["settings.history"] = () => Langs.Common.Resources.History,
        ["settings.more"] = () => Langs.Common.Resources.MoreSettings,
        ["settings.update"] = () => Langs.Common.Resources.UpdateSettings,
        ["settings.about"] = () => Langs.Common.Resources.About
    };
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        _startupConfigFilePath = Utils.GetFilePath("Config.json");
        _startupUiLanguageMode = TryLoadUiLanguageMode();
        _startupCulture = GetStartupCulture(_startupUiLanguageMode);
        InitializeLanguages(_startupCulture);
        BuildHost();
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();

            _mainWindow = new MainWindow
            {
                Content = IAppHost.GetService<MainView>(),
                Title = "SecRandom"
            };
            _mainWindow.Closed += (_, _) => _mainWindow = null;
            desktop.MainWindow = _mainWindow;
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime)
        {
            throw new PlatformNotSupportedException();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static void DisableAvaloniaDataAnnotationValidation()
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

    private static void InitializeLanguages(CultureInfo cultureInfo)
    {
        CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
        CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
        Langs.Common.Resources.Culture = cultureInfo;
        Langs.Pages.RollCallPage.Resources.Culture = cultureInfo;
        Langs.SettingsPages.BasicSettingsPage.Resources.Culture = cultureInfo;
        Langs.SettingsPages.DrawSettingsPage.Resources.Culture = cultureInfo;
        Langs.SettingsPages.FloatingWindowPage.Resources.Culture = cultureInfo;
        Langs.SettingsPages.LinkageSettingsPage.Resources.Culture = cultureInfo;
    }

    private static CultureInfo GetStartupCulture(UiLanguageMode uiLanguageMode)
    {
        return uiLanguageMode switch
        {
            UiLanguageMode.English => new CultureInfo("en-us"),
            UiLanguageMode.ChineseSimplified => new CultureInfo("zh-hans"),
            _ => NormalizeCulture(CultureInfo.InstalledUICulture)
        };
    }

    public static void ApplyUiLanguageMode(UiLanguageMode uiLanguageMode)
    {
        var culture = GetStartupCulture(uiLanguageMode);
        InitializeLanguages(culture);
        UpdateRegisteredPageNames();
        ReloadOpenWindows(uiLanguageMode, culture);
    }

    public static void ApplyUiThemeModeIndex(int uiThemeModeIndex)
    {
        if (Current is null)
        {
            return;
        }

        Current.RequestedThemeVariant = uiThemeModeIndex switch
        {
            1 => ThemeVariant.Light,
            2 => ThemeVariant.Dark,
            _ => ThemeVariant.Default
        };
    }

    public static void ApplyUiFont(string? uiFontFamilyName, int uiFontFamilyIndex, int uiFontWeightIndex)
    {
        if (Current is null)
        {
            return;
        }

        var fontFamilyName = ResolveUiFontFamilyName(uiFontFamilyName, uiFontFamilyIndex);

        var weightIndex = uiFontWeightIndex;
        if (weightIndex < 0 || weightIndex >= _uiFontWeights.Length)
        {
            weightIndex = 0;
        }

        Current.Resources["AppFontFamily"] = new FontFamily(fontFamilyName);
        Current.Resources["AppFontWeight"] = _uiFontWeights[weightIndex];
    }

    private static string ResolveUiFontFamilyName(string? uiFontFamilyName, int uiFontFamilyIndex)
    {
        if (!string.IsNullOrWhiteSpace(uiFontFamilyName))
        {
            return uiFontFamilyName;
        }

        if (uiFontFamilyIndex < 0 || uiFontFamilyIndex >= _legacyUiFontFamilies.Length)
        {
            return _legacyUiFontFamilies[0];
        }

        return _legacyUiFontFamilies[uiFontFamilyIndex];
    }

    private static void UpdateRegisteredPageNames()
    {
        foreach (var info in PagesRegistryService.MainItems)
        {
            UpdateRegisteredPageName(info);
        }

        foreach (var info in PagesRegistryService.SettingsItems)
        {
            UpdateRegisteredPageName(info);
        }
    }

    private static void UpdateRegisteredPageName(SecRandom.Core.Attributes.PageInfo info)
    {
        if (info.IsSeparator)
        {
            return;
        }

        if (_pageNameProviders.TryGetValue(info.Id, out var provider))
        {
            info.Name = provider();
        }
    }

    private static void ReloadOpenWindows(UiLanguageMode uiLanguageMode, CultureInfo culture)
    {
        _startupUiLanguageMode = uiLanguageMode;
        _startupCulture = culture;

        if (_mainWindow is { IsLoaded: true })
        {
            var selectedId = (_mainWindow.Content as MainView)?.ViewModel.SelectedPageInfo?.Id;
            var view = IAppHost.GetService<MainView>();
            _mainWindow.Content = view;
            if (!string.IsNullOrWhiteSpace(selectedId))
            {
                view.SelectNavigationItemById(selectedId);
            }
        }

        if (_settingsWindow is { IsLoaded: true })
        {
            var selectedId = (_settingsWindow.Content as SettingsView)?.ViewModel.SelectedPageInfo?.Id;
            var view = IAppHost.GetService<SettingsView>();
            _settingsWindow.Content = view;
            if (!string.IsNullOrWhiteSpace(selectedId))
            {
                view.SelectNavigationItemById(selectedId);
            }
        }
    }

    private static UiLanguageMode TryLoadUiLanguageMode()
    {
        try
        {
            var filePath = Utils.GetFilePath("Config.json");
            if (!File.Exists(filePath))
            {
                return UiLanguageMode.ChineseSimplified;
            }

            using var stream = File.OpenRead(filePath);
            using var doc = JsonDocument.Parse(stream);
            var element = default(JsonElement?);
            foreach (var property in doc.RootElement.EnumerateObject())
            {
                if (property.NameEquals("BasicSettings") ||
                    property.Name.Equals("BasicSettings", StringComparison.OrdinalIgnoreCase))
                {
                    if (property.Value.ValueKind != JsonValueKind.Object)
                    {
                        continue;
                    }

                    foreach (var subProperty in property.Value.EnumerateObject())
                    {
                        if (subProperty.NameEquals("UiLanguageMode") ||
                            subProperty.Name.Equals("UiLanguageMode", StringComparison.OrdinalIgnoreCase))
                        {
                            element = subProperty.Value;
                            break;
                        }
                    }

                    if (element is not null)
                    {
                        break;
                    }
                }

                if (property.NameEquals("UiLanguageMode") ||
                    property.Name.Equals("UiLanguageMode", StringComparison.OrdinalIgnoreCase))
                {
                    element = property.Value;
                    break;
                }
            }

            if (element is { ValueKind: JsonValueKind.Number } value && value.TryGetInt32(out var mode))
            {
                return (UiLanguageMode)mode;
            }

            return UiLanguageMode.ChineseSimplified;
        }
        catch
        {
            return UiLanguageMode.ChineseSimplified;
        }
    }

    private static CultureInfo NormalizeCulture(CultureInfo cultureInfo)
    {
        if (cultureInfo.Name.StartsWith("zh", StringComparison.OrdinalIgnoreCase))
        {
            return new CultureInfo("zh-hans");
        }

        return new CultureInfo("en-us");
    }
    
    private static void BuildHost()
    {
        if (IAppHost.Host is not null)
        {
            return;
        }

        IAppHost.Host = Host
            .CreateDefaultBuilder()
            .UseContentRoot(AppContext.BaseDirectory)
            .ConfigureServices(services =>
            {
                // 日志
                services.AddLogging(builder =>
                {
                    builder.AddConsoleFormatter<SecRandomConsoleFormatter, ConsoleFormatterOptions>();
                    builder.AddConsole(console => { console.FormatterName = "secrandom"; });
                    builder.AddProvider(new SecRandomFileLoggerProvider(Utils.GetFilePath("logs", "app.log")));
#if DEBUG
                    builder.SetMinimumLevel(LogLevel.Trace);
#endif
                });
                
                // 配置
                services.AddSingleton<ConfigServiceBase, DesktopConfigService>();
                services.AddSingleton<RootConfigHandler>();
                
                // 服务
                
                // 窗口
                services.AddTransient<MainView>();
                services.AddTransient<MainViewModel>();
                
                services.AddTransient<SettingsView>();
                services.AddTransient<SettingsViewModel>();
                
                // 界面 Views
                services.AddMainPage<RollCallPage>(Langs.Common.Resources.RollCall);
                
                services.AddSettingsPage<BasicSettingsPage>(Langs.Common.Resources.BasicSettings);
                services.AddSettingsPage<RosterManagementPage>(Langs.Common.Resources.RosterManagement);
                services.AddSettingsPage<RollCallSettingsSubPage>(Langs.SettingsPages.DrawSettingsPage.Resources.RollCallSettings);
                services.AddKeyedTransient<UserControl, RollCallListSpecificSettingsPage>("settings.draw.rollCall.listSpecific");
                services.AddSettingsPage<QuickDrawSettingsSubPage>(Langs.SettingsPages.DrawSettingsPage.Resources.QuickDrawSettings);
                services.AddKeyedTransient<UserControl, QuickDrawListSpecificSettingsPage>("settings.draw.quickDraw.listSpecific");
                services.AddSettingsPage<LotterySettingsSubPage>(Langs.SettingsPages.DrawSettingsPage.Resources.LotterySettings);
                services.AddKeyedTransient<UserControl, LotteryListSpecificSettingsPage>("settings.draw.lottery.listSpecific");
                services.AddSettingsPage<FaceDetectorSettingsSubPage>(Langs.SettingsPages.DrawSettingsPage.Resources.FaceDetectorSettings);
                services.AddSettingsPage<FloatingWindowPage>(Langs.Common.Resources.FloatingWindowManagement);
                services.AddSettingsPage<NotificationSettingsPage>(Langs.Common.Resources.NotificationSettings);
                services.AddSettingsPage<SecuritySettingsPage>(Langs.Common.Resources.SecuritySettings);
                services.AddSettingsPage<LinkageSettingsPage>(Langs.Common.Resources.LinkageSettings);
                services.AddSettingsPage<VoiceSettingsPage>(Langs.Common.Resources.VoiceSettings);
                services.AddSettingsPage<ThemeManagementPage>(Langs.Common.Resources.ThemeManagement);
                services.AddSettingsPage<HistoryPage>(Langs.Common.Resources.History);
                
                services.AddSettingsPage<MoreSettingsPage>(Langs.Common.Resources.MoreSettings);
                services.AddSettingsPage<UpdateSettingsPage>(Langs.Common.Resources.UpdateSettings);
                services.AddSettingsPage<AboutPage>(Langs.Common.Resources.About);

                // 界面 ViewModels
            })
            .Build();

        var logger = IAppHost.GetService<ILogger<App>>();
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("SecRandom  Copyright by SECTL(2025~{YEAR})  Licensed under GPL3.0",
                DateTime.Now.Year);
            logger.LogInformation("Host built.");
            if (_startupCulture is not null && !string.IsNullOrWhiteSpace(_startupConfigFilePath))
            {
                logger.LogInformation("UI Culture: {CULTURE} (Mode={MODE}, Config={PATH})",
                    _startupCulture.Name, _startupUiLanguageMode, _startupConfigFilePath);
            }
        }
        
        var basicSettings = IAppHost.GetService<RootConfigHandler>().Data.BasicSettings;
        ApplyUiThemeModeIndex(basicSettings.UiThemeModeIndex);
        ApplyUiFont(basicSettings.UiFontFamilyName, basicSettings.UiFontFamilyIndex, basicSettings.UiFontWeightIndex);
    }

    public static void ShowMainWindow()
    {
        if (_mainWindow is { IsVisible: true })
        {
            _mainWindow.Activate();
            return;
        }

        if (_mainWindow is not { IsLoaded: true })
        {
            _mainWindow = new MainWindow
            {
                Content = IAppHost.GetService<MainView>(),
                Title = "SecRandom"
            };
            _mainWindow.Closed += (_, _) => _mainWindow = null;
        }

        _mainWindow.Show();
        _mainWindow.Activate();
    }
    
    public static void ShowSettingsWindow()
    {
        if (_settingsWindow is { IsVisible: true })
        {
            _settingsWindow.Activate();
            return;
        }
        
        if (_settingsWindow is not { IsLoaded: true })
        {
            _settingsWindow = new MainWindow
            {
                Content = IAppHost.GetService<SettingsView>(),
                Title = "SecRandom"
            };
            _settingsWindow.Closed += (_, _) => _settingsWindow = null;
        }

        _settingsWindow.Show();
        _settingsWindow.Activate();
    }
}
