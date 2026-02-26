using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SecRandom.Services.Config;

namespace SecRandom.ViewModels.ListManagementSubPages.LotterySubPages;

public partial class NameSettingsViewModel : ObservableObject
{
    private readonly MainConfigHandler _configHandler;
    private readonly ILogger<NameSettingsViewModel>? _logger;

    [ObservableProperty]
    private PrizeItem? _selectedPrize;

    [ObservableProperty]
    private string _newName = string.Empty;

    public ObservableCollection<PrizeItem> Prizes { get; } = [];

    public NameSettingsViewModel(MainConfigHandler configHandler, ILogger<NameSettingsViewModel>? logger = null)
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
    private void UpdateName()
    {
        if (SelectedPrize == null || string.IsNullOrEmpty(NewName))
        {
            return;
        }

        try
        {
            SelectedPrize.Name = NewName;
            // TODO: 保存到服务
            _logger?.LogInformation("奖品名称已更新: {Name}", NewName);
        }
        catch (System.Exception ex)
        {
            _logger?.LogError(ex, "更新奖品名称失败");
        }
    }

    [RelayCommand]
    private void Save()
    {
        try
        {
            // TODO: 保存所有更改
            _logger?.LogInformation("名称设置已保存");
        }
        catch (System.Exception ex)
        {
            _logger?.LogError(ex, "保存名称设置失败");
        }
    }
}
