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

[PageInfo("settings.listManagement.lottery", "\uE8F1", "settings.listManagement")]
public partial class LotteryListSettingsSubPage : UserControl
{
    public LotteryListSettingsSubPage()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void OpenPreviewTable_OnClick(object? sender, RoutedEventArgs e)
    {
        SettingsView.Current?.SelectNavigationItemById("settings.listManagement.lottery.preview");
    }

    private void OpenSetPoolNameWindow_OnClick(object? sender, RoutedEventArgs e)
    {
    }

    private void OpenImportPrizeWindow_OnClick(object? sender, RoutedEventArgs e)
    {
    }

    private void OpenPrizeSettingsWindow_OnClick(object? sender, RoutedEventArgs e)
    {
    }

    private void OpenWeightSettingsWindow_OnClick(object? sender, RoutedEventArgs e)
    {
    }

    private void OpenCountSettingsWindow_OnClick(object? sender, RoutedEventArgs e)
    {
    }

    private void OpenExportPrizeWindow_OnClick(object? sender, RoutedEventArgs e)
    {
    }
}
