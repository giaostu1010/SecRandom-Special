using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SecRandom.Services.Config;

namespace SecRandom.ViewModels.ListManagementSubPages.LotterySubPages;

public partial class WeightSettingsViewModel : ObservableObject
{
    private readonly MainConfigHandler _configHandler;
    private readonly ILogger<WeightSettingsViewModel>? _logger;

    [ObservableProperty]
    private PrizeItem? _selectedPrize;

    public ObservableCollection<PrizeItem> Prizes { get; } = [];

    public WeightSettingsViewModel(MainConfigHandler configHandler, ILogger<WeightSettingsViewModel>? logger = null)
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
            _logger?.LogInformation("权重设置已保存");
        }
        catch (System.Exception ex)
        {
            _logger?.LogError(ex, "保存权重设置失败");
        }
    }
}
