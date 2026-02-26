using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SecRandom.Services.Config;

namespace SecRandom.ViewModels.ListManagementSubPages.ListSettingsSubPage;

public partial class LotteryListSettingsViewModel : ObservableObject
{
    private readonly MainConfigHandler _configHandler;
    private readonly ILogger<LotteryListSettingsViewModel>? _logger;

    public LotteryListSettingsViewModel(MainConfigHandler configHandler, ILogger<LotteryListSettingsViewModel>? logger = null)
    {
        _configHandler = configHandler;
        _logger = logger;
    }

    [RelayCommand]
    private void OpenPreviewTable()
    {
        // TODO: 导航到预览表格页面
        _logger?.LogInformation("打开预览表格");
    }

    [RelayCommand]
    private void OpenSetPoolName()
    {
        // TODO: 导航到设置奖池名称页面
        _logger?.LogInformation("打开设置奖池名称");
    }

    [RelayCommand]
    private void OpenImportPrize()
    {
        // TODO: 导航到导入奖品页面
        _logger?.LogInformation("打开导入奖品");
    }

    [RelayCommand]
    private void OpenPrizeSettings()
    {
        // TODO: 导航到奖品设置页面
        _logger?.LogInformation("打开奖品设置");
    }

    [RelayCommand]
    private void OpenWeightSettings()
    {
        // TODO: 导航到权重设置页面
        _logger?.LogInformation("打开权重设置");
    }

    [RelayCommand]
    private void OpenCountSettings()
    {
        // TODO: 导航到数量设置页面
        _logger?.LogInformation("打开数量设置");
    }

    [RelayCommand]
    private void OpenTagSettings()
    {
        // TODO: 导航到标签设置页面
        _logger?.LogInformation("打开标签设置");
    }

    [RelayCommand]
    private void OpenGenderSettings()
    {
        // TODO: 导航到性别设置页面
        _logger?.LogInformation("打开性别设置");
    }

    [RelayCommand]
    private void OpenGroupSettings()
    {
        // TODO: 导航到分组设置页面
        _logger?.LogInformation("打开分组设置");
    }

    [RelayCommand]
    private void OpenNameSettings()
    {
        // TODO: 导航到名称设置页面
        _logger?.LogInformation("打开名称设置");
    }

    [RelayCommand]
    private void OpenExportPrize()
    {
        // TODO: 导航到导出奖品页面
        _logger?.LogInformation("打开导出奖品");
    }
}
