using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using SecRandom.Core.Abstraction;
using SecRandom.Core.Attributes;
using SecRandom.Core.Services;
using SecRandom.Views;

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
    }

    private void OpenImportStudentWindow_OnClick(object? sender, RoutedEventArgs e)
    {
    }

    private void OpenNameSettingsWindow_OnClick(object? sender, RoutedEventArgs e)
    {
    }

    private void OpenGenderSettingsWindow_OnClick(object? sender, RoutedEventArgs e)
    {
    }

    private void OpenGroupSettingsWindow_OnClick(object? sender, RoutedEventArgs e)
    {
    }

    private void OpenTagSettingsWindow_OnClick(object? sender, RoutedEventArgs e)
    {
    }

    private void OpenExportStudentWindow_OnClick(object? sender, RoutedEventArgs e)
    {
    }
}
