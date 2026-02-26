using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SecRandom.Services.Config;

namespace SecRandom.ViewModels.ListManagementSubPages.LotterySubPages;

public partial class PrizeSettingsViewModel : ObservableObject
{
    private readonly MainConfigHandler _configHandler;
    private readonly ILogger<PrizeSettingsViewModel>? _logger;

    [ObservableProperty]
    private PrizeItem? _selectedPrize;

    [ObservableProperty]
    private string _newPrizeName = string.Empty;

    public ObservableCollection<PrizeItem> Prizes { get; } = new();

    public PrizeSettingsViewModel(MainConfigHandler configHandler, ILogger<PrizeSettingsViewModel>? logger = null)
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
    private void AddPrize()
    {
        if (string.IsNullOrEmpty(NewPrizeName))
        {
            return;
        }

        try
        {
            Prizes.Add(new PrizeItem { Name = NewPrizeName });
            NewPrizeName = string.Empty;
            _logger?.LogInformation("奖品已添加");
        }
        catch (System.Exception ex)
        {
            _logger?.LogError(ex, "添加奖品失败");
        }
    }

    [RelayCommand]
    private void DeleteSelected()
    {
        if (SelectedPrize == null)
        {
            _logger?.LogWarning("请先选择要删除的奖品");
            return;
        }

        try
        {
            Prizes.Remove(SelectedPrize);
            SelectedPrize = null;
            _logger?.LogInformation("奖品已删除");
        }
        catch (System.Exception ex)
        {
            _logger?.LogError(ex, "删除奖品失败");
        }
    }

    [RelayCommand]
    private void Save()
    {
        try
        {
            // TODO: 保存所有更改
            _logger?.LogInformation("奖品设置已保存");
        }
        catch (System.Exception ex)
        {
            _logger?.LogError(ex, "保存奖品设置失败");
        }
    }
}

public partial class PrizeItem : ObservableObject
{
    [ObservableProperty]
    private string _id = string.Empty;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private int _weight = 1;

    [ObservableProperty]
    private int _count = 1;

    [ObservableProperty]
    private string _tags = string.Empty;

    [ObservableProperty]
    private bool _isEnabled = true;
}
