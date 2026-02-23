using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using SecRandom.Core.Attributes;
using SecRandom.Core.Abstraction;
using SecRandom.Core.Helpers.UI;
using SecRandom.Models.Config;
using SecRandom.Services.Config;

namespace SecRandom.Views.SettingsPages;

[PageInfo("settings.linkageSettings", "\ue303")]
public partial class LinkageSettingsPage : UserControl
{
    public LinkageSettingsConfig ViewModel { get; } =
        IAppHost.GetService<RootConfigHandler>().Data.LinkageSettings;

    public LinkageSettingsPage()
    {
        DataContext = ViewModel;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void ImportFromCsesFile_OnClick(object? sender, RoutedEventArgs e)
    {
        this.ShowWarningToast(Langs.SettingsPages.LinkageSettingsPage.Resources.FeatureInDevelopment);
    }

    private void OpenCurrentCsesConfig_OnClick(object? sender, RoutedEventArgs e)
    {
        this.ShowWarningToast(Langs.SettingsPages.LinkageSettingsPage.Resources.FeatureInDevelopment);
    }

    private void ClearCsesSchedule_OnClick(object? sender, RoutedEventArgs e)
    {
        this.ShowWarningToast(Langs.SettingsPages.LinkageSettingsPage.Resources.FeatureInDevelopment);
    }
}
