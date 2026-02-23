using Avalonia.Controls;
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
