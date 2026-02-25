using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using SecRandom.Core.Attributes;

namespace SecRandom.Views.SettingsPages.ListManagementSubPages;

[PageInfo("settings.listManagement.rollCall", "\uE77B", "settings.listManagement")]
public partial class RollCallListSettingsSubPage : UserControl
{
    public RollCallListSettingsSubPage()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void OpenPreviewTable_OnClick(object? sender, RoutedEventArgs e)
    {
        SettingsView.Current?.SelectNavigationItemById("settings.listManagement.rollCall.preview");
    }

    private void OpenSetClassNameWindow_OnClick(object? sender, RoutedEventArgs e)
    {
        SettingsView.Current?.SelectNavigationItemById("settings.listManagement.rollCall.setClassName");
    }

    private void OpenImportStudentWindow_OnClick(object? sender, RoutedEventArgs e)
    {
        SettingsView.Current?.SelectNavigationItemById("settings.listManagement.rollCall.importStudent");
    }

    private void OpenNameSettingsWindow_OnClick(object? sender, RoutedEventArgs e)
    {
        SettingsView.Current?.SelectNavigationItemById("settings.listManagement.rollCall.nameSettings");
    }

    private void OpenGenderSettingsWindow_OnClick(object? sender, RoutedEventArgs e)
    {
        SettingsView.Current?.SelectNavigationItemById("settings.listManagement.rollCall.genderSettings");
    }

    private void OpenGroupSettingsWindow_OnClick(object? sender, RoutedEventArgs e)
    {
        SettingsView.Current?.SelectNavigationItemById("settings.listManagement.rollCall.groupSettings");
    }

    private void OpenTagSettingsWindow_OnClick(object? sender, RoutedEventArgs e)
    {
        SettingsView.Current?.SelectNavigationItemById("settings.listManagement.rollCall.tagSettings");
    }

    private void OpenExportStudentWindow_OnClick(object? sender, RoutedEventArgs e)
    {
        SettingsView.Current?.SelectNavigationItemById("settings.listManagement.rollCall.exportStudent");
    }
}
