using System;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using SecRandom.Core.Abstraction;
using SecRandom.Core.Enums;
using SecRandom.Models;
using SecRandom.Models.Config;

namespace SecRandom.Services.Config;

public class MainConfigHandler : ConfigHandlerBase<MainConfigModel>
{
    private DrawSettingsConfig? _drawSettings;
    private RollCallSettingsConfig? _rollCallSettings;
    private QuickDrawSettingsConfig? _quickDrawSettings;
    private LotterySettingsConfig? _lotterySettings;
    private FaceDetectorSettingsConfig? _faceDetectorSettings;

    public MainConfigHandler(ILogger<MainConfigHandler> logger, ConfigServiceBase configService)
        : base(logger, configService, () => new MainConfigModel())
    {
        AttachBasicSettingsHandlers();
        AttachDrawSettingsHandlers();
        AttachFloatingWindowSettingsHandlers();
    }

    protected override void Reload()
    {
        DetachBasicSettingsHandlers();
        DetachDrawSettingsHandlers();
        DetachFloatingWindowSettingsHandlers();
        base.Reload();
        AttachBasicSettingsHandlers();
        AttachDrawSettingsHandlers();
        AttachFloatingWindowSettingsHandlers();
    }

    protected override void Data_OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainConfigModel.BasicSettings))
        {
            DetachBasicSettingsHandlers();
            AttachBasicSettingsHandlers();
        }
        else if (e.PropertyName == nameof(MainConfigModel.FloatingWindowSettings))
        {
            DetachFloatingWindowSettingsHandlers();
            AttachFloatingWindowSettingsHandlers();
        }
        else if (e.PropertyName == nameof(MainConfigModel.DrawSettings))
        {
            DetachDrawSettingsHandlers();
            AttachDrawSettingsHandlers();
        }

        base.Data_OnPropertyChanged(sender, e);
    }

    private void AttachBasicSettingsHandlers()
    {
        Data.BasicSettings.PropertyChanged += BasicSettings_OnPropertyChanged;
    }

    private void DetachBasicSettingsHandlers()
    {
        Data.BasicSettings.PropertyChanged -= BasicSettings_OnPropertyChanged;
    }

    private void BasicSettings_OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(BasicSettingsConfig.UiLanguageMode))
        {
            App.ApplyUiLanguageMode(Data.BasicSettings.UiLanguageMode);
        }
        else if (e.PropertyName == nameof(BasicSettingsConfig.UiThemeModeIndex))
        {
            App.ApplyUiThemeModeIndex(Data.BasicSettings.UiThemeModeIndex);
        }
        else if (e.PropertyName == nameof(BasicSettingsConfig.UiFontFamilyName) ||
                 e.PropertyName == nameof(BasicSettingsConfig.UiFontFamilyIndex) ||
                 e.PropertyName == nameof(BasicSettingsConfig.UiFontWeightIndex))
        {
            App.ApplyUiFont(
                Data.BasicSettings.UiFontFamilyName,
                Data.BasicSettings.UiFontFamilyIndex,
                Data.BasicSettings.UiFontWeightIndex);
        }

        Save();
    }

    private void AttachDrawSettingsHandlers()
    {
        _drawSettings = Data.DrawSettings;
        _drawSettings.PropertyChanged += DrawSettings_OnPropertyChanged;

        _rollCallSettings = _drawSettings.RollCallSettings;
        _rollCallSettings.PropertyChanged += DrawSettingsChild_OnPropertyChanged;

        _quickDrawSettings = _drawSettings.QuickDrawSettings;
        _quickDrawSettings.PropertyChanged += DrawSettingsChild_OnPropertyChanged;

        _lotterySettings = _drawSettings.LotterySettings;
        _lotterySettings.PropertyChanged += DrawSettingsChild_OnPropertyChanged;

        _faceDetectorSettings = _drawSettings.FaceDetectorSettings;
        _faceDetectorSettings.PropertyChanged += DrawSettingsChild_OnPropertyChanged;
    }

    private void DetachDrawSettingsHandlers()
    {
        if (_drawSettings is not null)
        {
            _drawSettings.PropertyChanged -= DrawSettings_OnPropertyChanged;
            _drawSettings = null;
        }

        if (_rollCallSettings is not null)
        {
            _rollCallSettings.PropertyChanged -= DrawSettingsChild_OnPropertyChanged;
            _rollCallSettings = null;
        }

        if (_quickDrawSettings is not null)
        {
            _quickDrawSettings.PropertyChanged -= DrawSettingsChild_OnPropertyChanged;
            _quickDrawSettings = null;
        }

        if (_lotterySettings is not null)
        {
            _lotterySettings.PropertyChanged -= DrawSettingsChild_OnPropertyChanged;
            _lotterySettings = null;
        }

        if (_faceDetectorSettings is not null)
        {
            _faceDetectorSettings.PropertyChanged -= DrawSettingsChild_OnPropertyChanged;
            _faceDetectorSettings = null;
        }
    }

    private void DrawSettings_OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(DrawSettingsConfig.RollCallSettings) or
            nameof(DrawSettingsConfig.QuickDrawSettings) or
            nameof(DrawSettingsConfig.LotterySettings) or
            nameof(DrawSettingsConfig.FaceDetectorSettings))
        {
            DetachDrawSettingsHandlers();
            AttachDrawSettingsHandlers();
        }

        Save();
    }

    private void DrawSettingsChild_OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        Save();
    }

    private void AttachFloatingWindowSettingsHandlers()
    {
        Data.FloatingWindowSettings.PropertyChanged += FloatingWindowSettings_OnPropertyChanged;
    }

    private void DetachFloatingWindowSettingsHandlers()
    {
        Data.FloatingWindowSettings.PropertyChanged -= FloatingWindowSettings_OnPropertyChanged;
    }

    private void FloatingWindowSettings_OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        Save();
    }
}
