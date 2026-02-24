using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using SecRandom.Core.Attributes;
using SecRandom.Core.Abstraction;
using SecRandom.Models.Config;
using SecRandom.Services.Config;

namespace SecRandom.Views.SettingsPages;

[PageInfo("settings.floatingWindow", "\uf48a")]
public partial class FloatingWindowPage : UserControl
{
    public FloatingWindowSettingsConfig ViewModel { get; } =
        IAppHost.GetService<MainConfigHandler>().Data.FloatingWindowSettings;

    public static List<string> FloatingControls { get; } =
        ["roll_call", "quick_draw", "lottery", "face_draw", "timer"];
    public static readonly FuncValueConverter<string, string> FloatingControlLocalizationConverter =
        new(x => x switch {
            "roll_call" => Langs.SettingsPages.FloatingWindowPage.Resources.RollCallButton,
            "quick_draw" => Langs.SettingsPages.FloatingWindowPage.Resources.QuickDrawButton,
            "lottery" => Langs.SettingsPages.FloatingWindowPage.Resources.LotteryButton,
            "face_draw" => Langs.SettingsPages.FloatingWindowPage.Resources.FaceDrawButton,
            "timer" => Langs.SettingsPages.FloatingWindowPage.Resources.TimerButton,
            _ => "???"
        });

    public FloatingWindowPage()
    {
        DataContext = ViewModel;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void ResetFloatingWindowPosition_OnClick(object? sender, RoutedEventArgs e)
    {
        ViewModel.FloatingWindowPositionX = 100;
        ViewModel.FloatingWindowPositionY = 100;
    }
}
