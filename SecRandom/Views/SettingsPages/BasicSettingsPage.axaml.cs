using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Fonts;
using Avalonia.VisualTree;
using SecRandom.Core.Abstraction;
using SecRandom.Core.Attributes;
using SecRandom.Core.Helpers.UI;
using SecRandom.Models.Config;
using SecRandom.Services.Config;
using SecRandom.Views;

namespace SecRandom.Views.SettingsPages;

[PageInfo("settings.basic", "\uf4c4")]
public partial class BasicSettingsPage : UserControl
{
    public BasicSettingsConfig ViewModel { get; } = IAppHost.GetService<MainConfigHandler>().Data.BasicSettings;
    private ComboBox? _fontFamilyComboBox;
    private List<FontFamily> _fontFamilies = [];
    private static readonly string[] LegacyUiFontFamilies = ["HarmonyOS Sans SC", "Segoe UI", "Microsoft YaHei UI"];
    
    public BasicSettingsPage()
    {
        DataContext = ViewModel;
        InitializeComponent();
        _fontFamilyComboBox = this.FindControl<ComboBox>("FontFamilyComboBox");
        InitializeFontFamilies();
        ViewModel.PropertyChanged += ViewModel_OnPropertyChanged;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void InitializeFontFamilies()
    {
        if (_fontFamilyComboBox is null)
        {
            return;
        }

        var fontCollection = FontManager.Current.SystemFonts;
        _fontFamilies = new List<FontFamily>(fontCollection).OrderBy(x => x.Name).ToList();
        _fontFamilyComboBox.ItemsSource = _fontFamilies;
        SyncSelectedFontFamily();
    }

    private void ViewModel_OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(BasicSettingsConfig.UiFontFamilyName))
        {
            SyncSelectedFontFamily();
        }
    }

    private void SyncSelectedFontFamily()
    {
        if (_fontFamilyComboBox is null)
        {
            return;
        }

        if (_fontFamilies.Count == 0)
        {
            return;
        }

        var desiredName = ViewModel.UiFontFamilyName;
        if (string.IsNullOrWhiteSpace(desiredName))
        {
            var legacyIndex = ViewModel.UiFontFamilyIndex;
            if (legacyIndex < 0 || legacyIndex >= LegacyUiFontFamilies.Length)
            {
                legacyIndex = 0;
            }
            desiredName = LegacyUiFontFamilies[legacyIndex];
        }
        var selected = _fontFamilies.FirstOrDefault(x =>
                           string.Equals(x.Name, desiredName, StringComparison.OrdinalIgnoreCase)) ??
                       _fontFamilies[0];

        if (!Equals(_fontFamilyComboBox.SelectedItem, selected))
        {
            _fontFamilyComboBox.SelectedItem = selected;
        }

        if (!string.Equals(ViewModel.UiFontFamilyName, selected.Name, StringComparison.Ordinal))
        {
            ViewModel.UiFontFamilyName = selected.Name;
        }
    }

    private void FontFamilyComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is not ComboBox comboBox || comboBox.SelectedItem is not FontFamily selected)
        {
            return;
        }

        if (!string.Equals(ViewModel.UiFontFamilyName, selected.Name, StringComparison.Ordinal))
        {
            ViewModel.UiFontFamilyName = selected.Name;
        }
    }

    private void OpenThemeManagement_OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var settingsView = this.GetVisualAncestors().OfType<SettingsView>().FirstOrDefault();
        if (settingsView is null)
        {
            this.ShowWarningToast(Langs.SettingsPages.BasicSettingsPage.Resources.FeatureInDevelopment);
            return;
        }

        settingsView.SelectNavigationItemById("settings.themeManagement");
    }

    private void OpenLogViewer_OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        this.ShowWarningToast(Langs.SettingsPages.BasicSettingsPage.Resources.FeatureInDevelopment);
    }

    private void OpenBackupManager_OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        this.ShowWarningToast(Langs.SettingsPages.BasicSettingsPage.Resources.FeatureInDevelopment);
    }

    private void ExportDiagnosticData_OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        this.ShowWarningToast(Langs.SettingsPages.BasicSettingsPage.Resources.FeatureInDevelopment);
    }

    private void ExportSettings_OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        this.ShowWarningToast(Langs.SettingsPages.BasicSettingsPage.Resources.FeatureInDevelopment);
    }

    private void ImportSettings_OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        this.ShowWarningToast(Langs.SettingsPages.BasicSettingsPage.Resources.FeatureInDevelopment);
    }

    private void ExportAllData_OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        this.ShowWarningToast(Langs.SettingsPages.BasicSettingsPage.Resources.FeatureInDevelopment);
    }

    private void ImportAllData_OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        this.ShowWarningToast(Langs.SettingsPages.BasicSettingsPage.Resources.FeatureInDevelopment);
    }
}
