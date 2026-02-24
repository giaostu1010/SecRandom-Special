using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SecRandom.Core.Abstraction;
using SecRandom.Core.Attributes;
using SecRandom.Core.Helpers.UI;
using SecRandom.Models.Config;
using SecRandom.Services.Config;

namespace SecRandom.Views.SettingsPages;

[PageInfo("settings.securitySettings", "\uef4e")]
public partial class SecuritySettingsPage : UserControl
{
    public SecuritySettingsConfig ViewModel { get; } = IAppHost.GetService<MainConfigHandler>().Data.SecuritySettings;

    public SecuritySettingsPage()
    {
        DataContext = ViewModel;
        InitializeComponent();
        ViewModel.PropertyChanged += ViewModel_OnPropertyChanged;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void ViewModel_OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
    }

    private void SetPassword_OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        this.ShowWarningToast("该功能正在开发中");
    }

    private void SetTotp_OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        this.ShowWarningToast("该功能正在开发中");
    }

    private void BindUsb_OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        this.ShowWarningToast("该功能正在开发中");
    }

    private void UnbindUsb_OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        this.ShowWarningToast("该功能正在开发中");
    }
}
