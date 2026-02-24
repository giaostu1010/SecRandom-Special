using System;
using System.Diagnostics;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using SecRandom.Core;
using SecRandom.Core.Abstraction;
using SecRandom.Core.Attributes;
using SecRandom.Core.Enums;
using SecRandom.Services.Config;

namespace SecRandom.Views.SettingsPages;

[PageInfo("settings.about", "\ue9e3", null, PageLocation.Bottom)]
public partial class AboutPage : UserControl, IDisposable
{
    private const string GITHUB_WEB = "https://github.com/SECTL/SecRandom";
    private const string BILIBILI_WEB = "https://space.bilibili.com/520571577";
    private const string WEBSITE = "https://secrandom.sectl.top";
    private const string SECTL_WEBSITE = "https://sectl.top";
    private const string DONATION_URL = "https://afdian.com/a/lzy0983";
    
    private int _baseRuntimeSeconds;
    private Timer? _runtimeTimer;
    private MainConfigHandler? _configHandler;
    private int _lastSavedSeconds;

    public AboutPage()
    {
        InitializeComponent();
        LoadVersionInfo();
        LoadUserInfo();
        StartRuntimeTimer();
    }

    private void InitializeComponent()
    {
        Avalonia.Markup.Xaml.AvaloniaXamlLoader.Load(this);
    }

    private void LoadVersionInfo()
    {
        var versionString = GlobalConstants.Version;
        
        var codename = GlobalConstants.Codename;
        var system = GetSystemInfo();
        var arch = GetArchitecture();
        
        if (this.FindControl<TextBlock>("VersionText") is { } versionText)
        {
            versionText.Text = $"{versionString} | {codename} ({system}-{arch})";
        }

        var currentYear = DateTime.Now.Year;
        var startYear = 2025;
        var displayYear = currentYear > startYear ? $"{startYear}-{currentYear}" : $"{startYear}";
        if (this.FindControl<TextBlock>("CopyrightText") is { } copyrightText)
        {
            copyrightText.Text = $"Copyright © {displayYear} SECTL";
        }
    }

    private void LoadUserInfo()
    {
        try
        {
            _configHandler = IAppHost.GetService<MainConfigHandler>();
            var config = _configHandler.Data;
            _baseRuntimeSeconds = config.UserSettings?.TotalRuntimeSeconds ?? 0;
            _lastSavedSeconds = _baseRuntimeSeconds;
            
            if (this.FindControl<TextBlock>("UserNameText") is { } userNameText)
            {
                userNameText.Text = Environment.UserName;
            }
            
            if (this.FindControl<TextBlock>("UserIdText") is { } userIdText)
            {
                userIdText.Text = config.UserSettings?.UserId ?? "N/A";
            }
            
            if (this.FindControl<TextBlock>("FirstUseTimeText") is { } firstUseTimeText)
            {
                firstUseTimeText.Text = config.UserSettings?.FirstUseTime ?? "N/A";
            }
            
            UpdateRuntimeDisplay();
            
            if (this.FindControl<TextBlock>("TotalDrawCountText") is { } totalDrawCountText)
            {
                totalDrawCountText.Text = (config.UserSettings?.TotalDrawCount ?? 0).ToString();
            }
            
            if (this.FindControl<TextBlock>("RollCallTotalCountText") is { } rollCallTotalCountText)
            {
                rollCallTotalCountText.Text = (config.UserSettings?.RollCallTotalCount ?? 0).ToString();
            }
            
            if (this.FindControl<TextBlock>("LotteryTotalCountText") is { } lotteryTotalCountText)
            {
                lotteryTotalCountText.Text = (config.UserSettings?.LotteryTotalCount ?? 0).ToString();
            }
        }
        catch
        {
        }
    }
    
    private void StartRuntimeTimer()
    {
        _runtimeTimer = new Timer(UpdateRuntimeCallback, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(500));
    }
    
    private void UpdateRuntimeCallback(object? state)
    {
        Dispatcher.UIThread.Post(() =>
        {
            try
            {
                var sessionSeconds = (int)(DateTime.Now - Process.GetCurrentProcess().StartTime).TotalSeconds;
                var totalSeconds = _baseRuntimeSeconds + sessionSeconds;
                
                if (this.FindControl<TextBlock>("RuntimeText") is { } runtimeText)
                {
                    runtimeText.Text = CalculateRuntime(totalSeconds);
                }
                
                if (_configHandler != null && totalSeconds - _lastSavedSeconds >= 30)
                {
                    _configHandler.Data.UserSettings!.TotalRuntimeSeconds = totalSeconds;
                    _configHandler.Save();
                    _lastSavedSeconds = totalSeconds;
                }
            }
            catch
            {
            }
        });
    }
    
    public void Dispose()
    {
        try
        {
            _runtimeTimer?.Dispose();

            if (_configHandler != null)
            {
                var sessionSeconds = (int)(DateTime.Now - Process.GetCurrentProcess().StartTime).TotalSeconds;
                var totalSeconds = _baseRuntimeSeconds + sessionSeconds;
                _configHandler.Data.UserSettings!.TotalRuntimeSeconds = totalSeconds;
                _configHandler.Save();
            }
        }
        catch
        {
        }

        GC.SuppressFinalize(this);
    }
    
    private void UpdateRuntimeDisplay()
    {
        if (this.FindControl<TextBlock>("RuntimeText") is { } runtimeText)
        {
            var sessionSeconds = (int)(DateTime.Now - Process.GetCurrentProcess().StartTime).TotalSeconds;
            var totalSeconds = _baseRuntimeSeconds + sessionSeconds;
            runtimeText.Text = CalculateRuntime(totalSeconds);
        }
    }

    private static string CalculateRuntime(int totalSeconds)
    {
        var years = totalSeconds / (365 * 24 * 3600);
        var remaining = totalSeconds % (365 * 24 * 3600);
        var days = remaining / (24 * 3600);
        remaining %= 24 * 3600;
        var hours = remaining / 3600;
        remaining %= 3600;
        var minutes = remaining / 60;
        var seconds = remaining % 60;
        
        var parts = new System.Collections.Generic.List<string>();
        if (years > 0)
            parts.Add($"{years}年");
        if (days > 0)
            parts.Add($"{days}天");
        parts.Add($"{hours:00}:{minutes:00}:{seconds:00}");
        
        return string.Join(" ", parts);
    }

    private static string GetSystemInfo()
    {
        if (OperatingSystem.IsWindows())
            return "Windows";
        if (OperatingSystem.IsMacOS())
            return "macOS";
        if (OperatingSystem.IsLinux())
            return "Linux";
        return Environment.OSVersion.Platform.ToString();
    }

    private static string GetArchitecture()
    {
        if (OperatingSystem.IsMacOS() || OperatingSystem.IsLinux())
        {
            var arch = System.Runtime.InteropServices.RuntimeInformation.OSArchitecture;
            var archStr = arch.ToString().ToLower();
            if (archStr.Contains("arm64") || archStr.Contains("aarch64"))
                return "arm64";
            return archStr.Replace("x86", "x64").Replace("amd64", "x64");
        }
        return Environment.Is64BitOperatingSystem ? "x64" : "x86";
    }

    private static void OpenUrl(string url)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
        catch
        {
        }
    }

    private void OpenBilibili_Click(object? sender, RoutedEventArgs e)
    {
        OpenUrl(BILIBILI_WEB);
    }

    private void OpenGitHub_Click(object? sender, RoutedEventArgs e)
    {
        OpenUrl(GITHUB_WEB);
    }

    private void OpenWebsite_Click(object? sender, RoutedEventArgs e)
    {
        OpenUrl(WEBSITE);
    }

    private void OpenOrganizationWebsite_Click(object? sender, RoutedEventArgs e)
    {
        OpenUrl(SECTL_WEBSITE);
    }

    private void OpenDonation_Click(object? sender, RoutedEventArgs e)
    {
        OpenUrl(DONATION_URL);
    }

    private void ShowContributors_Click(object? sender, RoutedEventArgs e)
    {
    }

    private async void CopyUserInfo_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel?.Clipboard != null)
            {
                var config = IAppHost.GetService<MainConfigHandler>().Data;
                var text = string.Join(Environment.NewLine, new[]
                {
                    $"{Langs.SettingsPages.AboutPage.Resources.UserName}: {Environment.UserName}",
                    $"{Langs.SettingsPages.AboutPage.Resources.UserId}: {config.UserSettings?.UserId ?? "N/A"}",
                    $"{Langs.SettingsPages.AboutPage.Resources.FirstUseTime}: {config.UserSettings?.FirstUseTime ?? "N/A"}",
                    $"{Langs.SettingsPages.AboutPage.Resources.Runtime}: {CalculateRuntime(config.UserSettings?.TotalRuntimeSeconds ?? 0)}",
                    $"{Langs.SettingsPages.AboutPage.Resources.TotalDrawCount}: {config.UserSettings?.TotalDrawCount ?? 0}",
                    $"{Langs.SettingsPages.AboutPage.Resources.RollCallTotalCount}: {config.UserSettings?.RollCallTotalCount ?? 0}",
                    $"{Langs.SettingsPages.AboutPage.Resources.LotteryTotalCount}: {config.UserSettings?.LotteryTotalCount ?? 0}"
                });
                await topLevel.Clipboard.SetTextAsync(text);
            }
        }
        catch
        {
        }
    }
}