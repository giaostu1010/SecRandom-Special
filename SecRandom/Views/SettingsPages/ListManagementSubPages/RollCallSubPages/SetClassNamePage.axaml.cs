using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using FluentAvalonia.UI.Controls;
using SecRandom.Core.Attributes;
using SecRandom.Core.Enums;

namespace SecRandom.Views.SettingsPages.ListManagementSubPages.RollCallSubPages;

[PageInfo("settings.listManagement.rollCall.setClassName", "\uE8EC", "settings.listManagement", PageLocation.Top, true)]
public partial class SetClassNamePage : UserControl
{
    public SetClassNamePage()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private async void CreateNewClass_OnClick(object? sender, RoutedEventArgs e)
    {
        // TODO: 实现创建新班级的逻辑
        await ShowMessageAsync("提示", "创建新班级功能待实现");
    }

    private async void RenameClass_OnClick(object? sender, RoutedEventArgs e)
    {
        // TODO: 实现重命名班级的逻辑
        await ShowMessageAsync("提示", "重命名班级功能待实现");
    }

    private async void DeleteClass_OnClick(object? sender, RoutedEventArgs e)
    {
        // TODO: 实现删除班级的逻辑
        await ShowMessageAsync("提示", "删除班级功能待实现");
    }

    private static async Task ShowMessageAsync(string title, string message)
    {
        var dialog = new ContentDialog
        {
            Title = title,
            Content = message,
            CloseButtonText = "确定"
        };
        await dialog.ShowAsync();
    }
}
