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
        NavigateToPage("settings.listManagement.rollCall.setClassName");
    }

    private void OpenImportStudentWindow_OnClick(object? sender, RoutedEventArgs e)
    {
        NavigateToPage("settings.listManagement.rollCall.importStudent");
    }

    private void OpenNameSettingsWindow_OnClick(object? sender, RoutedEventArgs e)
    {
        NavigateToPage("settings.listManagement.rollCall.nameSettings");
    }

    private void OpenGenderSettingsWindow_OnClick(object? sender, RoutedEventArgs e)
    {
        NavigateToPage("settings.listManagement.rollCall.genderSettings");
    }

    private void OpenGroupSettingsWindow_OnClick(object? sender, RoutedEventArgs e)
    {
        NavigateToPage("settings.listManagement.rollCall.groupSettings");
    }

    private void OpenTagSettingsWindow_OnClick(object? sender, RoutedEventArgs e)
    {
        NavigateToPage("settings.listManagement.rollCall.tagSettings");
    }

    private void OpenExportStudentWindow_OnClick(object? sender, RoutedEventArgs e)
    {
        NavigateToPage("settings.listManagement.rollCall.exportStudent");
    }

    private void NavigateToPage(string pageId)
    {
        var settingsView = this.GetVisualAncestors().OfType<SettingsView>().FirstOrDefault();
        var pageInfo = PagesRegistryService.SettingsItems.FirstOrDefault(x => x.Id == pageId);

        if (settingsView is not null && pageInfo is not null)
        {
            settingsView.NavigateToPage(pageInfo, false);
        }
        else if (settingsView is not null)
        {
            // 如果页面未注册，创建一个临时的 PageInfo
            var icon = pageId switch
            {
                "settings.listManagement.rollCall.setClassName" => "\uE8EC",
                "settings.listManagement.rollCall.importStudent" => "\uE8E5",
                "settings.listManagement.rollCall.nameSettings" => "\uE8A1",
                "settings.listManagement.rollCall.genderSettings" => "\uE7C3",
                "settings.listManagement.rollCall.groupSettings" => "\uE902",
                "settings.listManagement.rollCall.tagSettings" => "\uE8EC",
                "settings.listManagement.rollCall.exportStudent" => "\uEDE1",
                _ => "\uE8A1"
            };
            settingsView.NavigateToPage(
                new PageInfo(pageId, icon, "settings.listManagement"),
                false);
        }
    }
}
