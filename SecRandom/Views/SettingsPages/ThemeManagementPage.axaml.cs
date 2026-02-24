using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using SecRandom.Core.Abstraction;
using SecRandom.Core.Attributes;
using SecRandom.Models.Config;
using SecRandom.ViewModels;

namespace SecRandom.Views.SettingsPages;

[PageInfo("settings.themeManagement", "\uec49")]
public partial class ThemeManagementPage : UserControl
{
    public ViewModelBase ViewModel { get; } = IAppHost.GetService<ViewModelBase>();

    public ThemeManagementPage()
    {
        DataContext = ViewModel.Config.ThemeManagement;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private async void SelectMainWindowImage_Click(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null) return;

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "选择背景图片",
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("图片文件")
                {
                    Patterns = new[] { "*.png", "*.jpg", "*.jpeg", "*.bmp", "*.gif" }
                }
            }
        });

        if (files.Count > 0)
        {
            var config = DataContext as ThemeManagementSettingsConfig;
            if (config != null)
            {
                config.MainWindowBackgroundImage = files[0].Path.LocalPath;
            }
        }
    }

    private async void SelectSettingsWindowImage_Click(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null) return;

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "选择背景图片",
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("图片文件")
                {
                    Patterns = new[] { "*.png", "*.jpg", "*.jpeg", "*.bmp", "*.gif" }
                }
            }
        });

        if (files.Count > 0)
        {
            var config = DataContext as ThemeManagementSettingsConfig;
            if (config != null)
            {
                config.SettingsWindowBackgroundImage = files[0].Path.LocalPath;
            }
        }
    }

    private async void SelectNotificationWindowImage_Click(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null) return;

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "选择背景图片",
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("图片文件")
                {
                    Patterns = new[] { "*.png", "*.jpg", "*.jpeg", "*.bmp", "*.gif" }
                }
            }
        });

        if (files.Count > 0)
        {
            var config = DataContext as ThemeManagementSettingsConfig;
            if (config != null)
            {
                config.NotificationFloatingWindowBackgroundImage = files[0].Path.LocalPath;
            }
        }
    }
}
