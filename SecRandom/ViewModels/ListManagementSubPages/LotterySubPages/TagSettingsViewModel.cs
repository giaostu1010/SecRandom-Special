using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SecRandom.Services.Config;

namespace SecRandom.ViewModels.ListManagementSubPages.LotterySubPages;

public partial class TagSettingsViewModel : ObservableObject
{
    private readonly MainConfigHandler _configHandler;
    private readonly ILogger<TagSettingsViewModel>? _logger;

    [ObservableProperty]
    private PrizeItem? _selectedPrize;

    [ObservableProperty]
    private string _newTag = string.Empty;

    public ObservableCollection<PrizeItem> Prizes { get; } = new();

    public ObservableCollection<string> AvailableTags { get; } = new();

    public TagSettingsViewModel(MainConfigHandler configHandler, ILogger<TagSettingsViewModel>? logger = null)
    {
        _configHandler = configHandler;
        _logger = logger;
        LoadPrizes();
        LoadAvailableTags();
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

    private void LoadAvailableTags()
    {
        try
        {
            // TODO: 从服务加载可用标签
            _logger?.LogInformation("可用标签已加载");
        }
        catch (System.Exception ex)
        {
            _logger?.LogError(ex, "加载可用标签失败");
        }
    }

    [RelayCommand]
    private void AddTag()
    {
        if (SelectedPrize == null || string.IsNullOrEmpty(NewTag))
        {
            return;
        }

        try
        {
            // TODO: 添加标签到奖品
            _logger?.LogInformation("标签已添加: {Tag}", NewTag);
        }
        catch (System.Exception ex)
        {
            _logger?.LogError(ex, "添加标签失败");
        }
    }

    [RelayCommand]
    private void RemoveTag(string tag)
    {
        if (SelectedPrize == null || string.IsNullOrEmpty(tag))
        {
            return;
        }

        try
        {
            // TODO: 从奖品移除标签
            _logger?.LogInformation("标签已移除: {Tag}", tag);
        }
        catch (System.Exception ex)
        {
            _logger?.LogError(ex, "移除标签失败");
        }
    }

    [RelayCommand]
    private void Save()
    {
        try
        {
            // TODO: 保存所有更改
            _logger?.LogInformation("标签设置已保存");
        }
        catch (System.Exception ex)
        {
            _logger?.LogError(ex, "保存标签设置失败");
        }
    }
}
