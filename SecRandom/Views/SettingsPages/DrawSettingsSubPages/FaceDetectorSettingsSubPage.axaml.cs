using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using SecRandom.Core.Abstraction;
using SecRandom.Core.Attributes;
using SecRandom.Core;
using SecRandom.Core.Helpers.Media;
using pageLangs = SecRandom.Langs.SettingsPages.DrawSettingsPage.Resources;
using SecRandom.Models.Config;
using SecRandom.Services.Config;

namespace SecRandom.Views.SettingsPages;

[PageInfo("settings.draw.faceDetector", "\ue07c")]
public partial class FaceDetectorSettingsSubPage : UserControl
{
    private readonly FaceDetectorSettingsConfig _settings;
    private ComboBox? _cameraSourceComboBox;
    private ComboBox? _cameraResolutionComboBox;
    private ComboBox? _detectorTypeComboBox;
    private bool _isSyncing;
    private string _autoResolutionText = string.Empty;

    public FaceDetectorSettingsSubPage()
    {
        _settings = IAppHost.GetService<MainConfigHandler>().Data.DrawSettings.FaceDetectorSettings;
        DataContext = _settings;
        InitializeComponent();

        _cameraSourceComboBox = this.FindControl<ComboBox>("CameraSourceComboBox");
        _cameraResolutionComboBox = this.FindControl<ComboBox>("CameraResolutionComboBox");
        _detectorTypeComboBox = this.FindControl<ComboBox>("DetectorTypeComboBox");

        RefreshCameraList();
        RefreshResolutionList();
        RefreshModelList();

        _settings.PropertyChanged += Settings_OnPropertyChanged;
        DetachedFromVisualTree += FaceDetectorSettingsSubPage_OnDetachedFromVisualTree;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void FaceDetectorSettingsSubPage_OnDetachedFromVisualTree(object? sender, Avalonia.VisualTreeAttachmentEventArgs e)
    {
        DetachedFromVisualTree -= FaceDetectorSettingsSubPage_OnDetachedFromVisualTree;
        _settings.PropertyChanged -= Settings_OnPropertyChanged;
        _cameraSourceComboBox = null;
        _cameraResolutionComboBox = null;
        _detectorTypeComboBox = null;
    }

    private void Settings_OnPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(FaceDetectorSettingsConfig.CameraSourceIndex))
        {
            SyncCameraSelection();
            RefreshResolutionList();
            return;
        }

        if (e.PropertyName == nameof(FaceDetectorSettingsConfig.CameraDisplayResolution))
        {
            SyncResolutionSelection();
        }
    }

    private void CameraSourceComboBox_OnDropDownOpened(object? sender, EventArgs e)
    {
        RefreshCameraList();
    }

    private void CameraSourceComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_isSyncing || _cameraSourceComboBox is null)
        {
            return;
        }

        var index = _cameraSourceComboBox.SelectedIndex;
        if (index < 0)
        {
            return;
        }

        if (_settings.CameraSourceIndex != index)
        {
            _settings.CameraSourceIndex = index;
        }

        RefreshResolutionList();
    }

    private void CameraResolutionComboBox_OnDropDownOpened(object? sender, EventArgs e)
    {
        RefreshResolutionList();
    }

    private void CameraResolutionComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_isSyncing || _cameraResolutionComboBox is null)
        {
            return;
        }

        if (_cameraResolutionComboBox.SelectedItem is not string selected)
        {
            return;
        }

        if (string.Equals(selected, _autoResolutionText, StringComparison.Ordinal))
        {
            if (!string.IsNullOrWhiteSpace(_settings.CameraDisplayResolution))
            {
                _settings.CameraDisplayResolution = string.Empty;
            }
            return;
        }

        var value = selected.Trim();
        if (!string.Equals(_settings.CameraDisplayResolution, value, StringComparison.Ordinal))
        {
            _settings.CameraDisplayResolution = value;
        }
    }

    private void RefreshCameraList()
    {
        if (_cameraSourceComboBox is null)
        {
            return;
        }

        var cameras = CameraResolutionHelper.GetCameraNames();

        _isSyncing = true;
        _cameraSourceComboBox.ItemsSource = cameras;
        _isSyncing = false;

        if (cameras.Count > 0 && (_settings.CameraSourceIndex < 0 || _settings.CameraSourceIndex >= cameras.Count))
        {
            _settings.CameraSourceIndex = 0;
        }

        SyncCameraSelection();
    }

    private void SyncCameraSelection()
    {
        if (_cameraSourceComboBox is null)
        {
            return;
        }

        _isSyncing = true;
        if (_cameraSourceComboBox.ItemCount == 0)
        {
            _cameraSourceComboBox.SelectedIndex = -1;
        }
        else
        {
            _cameraSourceComboBox.SelectedIndex = _settings.CameraSourceIndex;
        }
        _isSyncing = false;
    }

    private void RefreshResolutionList()
    {
        if (_cameraResolutionComboBox is null)
        {
            return;
        }

        var configured = _settings.CameraDisplayResolution?.Trim() ?? string.Empty;
        var maximum = CameraResolutionHelper.GetMaximumResolutionByIndex(_settings.CameraSourceIndex);
        var maximumText = maximum.Width > 0 && maximum.Height > 0
            ? maximum.ToString()
            : pageLangs.CameraDisplayResolutionUnknown;
        _autoResolutionText = string.Format(pageLangs.CameraDisplayResolutionAutoFormat, maximumText);
        var autoText = _autoResolutionText;

        var items = CameraResolutionHelper.GetSuggestedResolutionsByIndex(_settings.CameraSourceIndex)
            .Select(x => x.ToString())
            .ToList();

        var options = new System.Collections.Generic.List<string>();
        if (!string.IsNullOrWhiteSpace(autoText))
        {
            options.Add(autoText);
        }

        options.AddRange(items);

        if (!string.IsNullOrWhiteSpace(configured) &&
            !options.Contains(configured, StringComparer.OrdinalIgnoreCase))
        {
            options.Add(configured);
        }

        _isSyncing = true;
        _cameraResolutionComboBox.ItemsSource = options;
        _isSyncing = false;

        SyncResolutionSelection();
    }

    private void SyncResolutionSelection()
    {
        if (_cameraResolutionComboBox is null)
        {
            return;
        }

        var configured = _settings.CameraDisplayResolution?.Trim() ?? string.Empty;
        var autoText = string.IsNullOrWhiteSpace(_autoResolutionText) ? string.Empty : _autoResolutionText;

        _isSyncing = true;
        if (string.IsNullOrWhiteSpace(configured))
        {
            _cameraResolutionComboBox.SelectedItem = autoText;
        }
        else
        {
            _cameraResolutionComboBox.SelectedItem = configured;
        }
        _isSyncing = false;
    }

    private void DetectorTypeComboBox_OnDropDownOpened(object? sender, EventArgs e)
    {
        RefreshModelList();
    }

    private void RefreshModelList()
    {
        if (_detectorTypeComboBox is null)
        {
            return;
        }

        var folderPath = Utils.GetFilePath("cv_models");
        Directory.CreateDirectory(folderPath);

        var preferred = (_detectorTypeComboBox.SelectedItem as string)?.Trim();
        if (string.IsNullOrWhiteSpace(preferred))
        {
            preferred = _settings.DetectorType?.Trim();
        }

        var items = Directory.EnumerateFiles(folderPath, "*.onnx", SearchOption.TopDirectoryOnly)
            .Select(Path.GetFileName)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
            .ToList();

        _detectorTypeComboBox.ItemsSource = items;

        if (!string.IsNullOrWhiteSpace(preferred) && items.Contains(preferred, StringComparer.OrdinalIgnoreCase))
        {
            _detectorTypeComboBox.SelectedItem = items.First(x => string.Equals(x, preferred, StringComparison.OrdinalIgnoreCase));
        }
    }

    private void OpenModelFolder_OnClick(object? sender, RoutedEventArgs e)
    {
        var folderPath = Utils.GetFilePath("cv_models");
        Directory.CreateDirectory(folderPath);
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = folderPath,
                UseShellExecute = true
            });
        }
        catch
        {
        }
    }
}
