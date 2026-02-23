using System.ComponentModel;
using Microsoft.Extensions.Logging;
using SecRandom.Core.Abstraction;
using SecRandom.Models;

namespace SecRandom.Services.Config;

public class MainConfigHandler(ILogger<MainConfigHandler> logger, ConfigServiceBase configService)
    : ConfigHandlerBase<MainConfigModel>(logger, configService, () => new MainConfigModel())
{
    protected override void Data_OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        base.Data_OnPropertyChanged(sender, e);

        if (e.PropertyName == nameof(MainConfigModel.UiLanguageMode))
        {
            App.ApplyUiLanguageMode(Data.UiLanguageMode);
            return;
        }

        if (e.PropertyName == nameof(MainConfigModel.UiThemeModeIndex))
        {
            App.ApplyUiThemeModeIndex(Data.UiThemeModeIndex);
            return;
        }

        if (e.PropertyName == nameof(MainConfigModel.UiFontFamilyName) ||
            e.PropertyName == nameof(MainConfigModel.UiFontFamilyIndex) ||
            e.PropertyName == nameof(MainConfigModel.UiFontWeightIndex))
        {
            App.ApplyUiFont(Data.UiFontFamilyName, Data.UiFontFamilyIndex, Data.UiFontWeightIndex);
        }
    }
}
