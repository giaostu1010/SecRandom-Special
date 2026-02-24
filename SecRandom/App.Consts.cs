using System;
using System.Collections.Generic;
using Avalonia.Media;

namespace SecRandom;

public partial class App
{
    private static readonly string[] LegacyUiFontFamilies = ["MiSans", "HarmonyOS Sans SC", "Segoe UI", "Microsoft YaHei UI"];
    private static readonly FontWeight[] UiFontWeights = [FontWeight.Thin, FontWeight.ExtraLight, FontWeight.Light, FontWeight.Normal, FontWeight.Medium, FontWeight.SemiBold, FontWeight.Bold, FontWeight.ExtraBold, FontWeight.Black, FontWeight.UltraBlack];
    private static readonly Dictionary<string, Func<string>> PageNameProviders = new()
    {
        ["main.rollCall"] = () => Langs.Common.Resources.RollCall,
        ["settings.basic"] = () => Langs.Common.Resources.BasicSettings,
        ["settings.listManagement.rollCall"] = () => Langs.SettingsPages.ListManagementPage.Resources.RollCallListSettings,
        ["settings.listManagement.lottery"] = () => Langs.SettingsPages.ListManagementPage.Resources.LotteryListSettings,
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
        ["settings.update"] = () => Langs.Common.Resources.UpdateSettings,
        ["settings.about"] = () => Langs.Common.Resources.About
    };
}