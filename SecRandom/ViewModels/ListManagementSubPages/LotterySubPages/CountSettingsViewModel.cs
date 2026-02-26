using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SecRandom.Services.Config;

namespace SecRandom.ViewModels.ListManagementSubPages.LotterySubPages;

public partial class CountSettingsViewModel : ObservableObject
{
    private readonly MainConfigHandler _configHandler;
    private readonly ILogger<CountSettingsViewModel>? _logger;

    [ObservableProperty]
    private PrizeItem? _selectedPrize;

    public ObservableCollection<PrizeItem> Prizes { get; } = new();

    public CountSettingsViewModel(MainConfigHandler configHandler, ILogger<CountSettingsViewModel>? logger = null)
    {
        _configHandler = configHandler;
        _logger = logger;
        LoadPrizes();
    }

    private void LoadPrizes()
    {
        try
        {
            // TODO: 从服务加载奖品列表
            _logger?.LogInformation("奖品列表已加载");
        }
        catch (System.Exception ex)
        {
            _logger?.LogError(ex, "加载奖品列表失败");
        }
    }

    [RelayCommand]
    private void Save()
    {
        try
        {
            // TODO: 保存所有更改
            _logger?.LogInformation("数量设置已保存");
        }
        catch (System.Exception ex)
        {
            _logger?.LogError(ex, "保存数量设置失败");
        }
    }
}
