using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SecRandom.Services.Config;

namespace SecRandom.ViewModels.ListManagementSubPages.LotterySubPages;

public partial class GroupSettingsViewModel : ObservableObject
{
    private readonly MainConfigHandler _configHandler;
    private readonly ILogger<GroupSettingsViewModel>? _logger;

    [ObservableProperty]
    private PrizeItem? _selectedPrize;

    [ObservableProperty]
    private string _newGroup = string.Empty;

    public ObservableCollection<PrizeItem> Prizes { get; } = new();

    public GroupSettingsViewModel(MainConfigHandler configHandler, ILogger<GroupSettingsViewModel>? logger = null)
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
            _logger?.LogInformation("分组设置已保存");
        }
        catch (System.Exception ex)
        {
            _logger?.LogError(ex, "保存分组设置失败");
        }
    }
}
