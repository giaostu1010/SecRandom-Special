using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SecRandom.Services.Config;

namespace SecRandom.ViewModels.ListManagementSubPages.LotterySubPages;

public partial class ImportPrizeViewModel : ObservableObject
{
    private readonly MainConfigHandler _configHandler;
    private readonly ILogger<ImportPrizeViewModel>? _logger;

    [ObservableProperty]
    private string _selectedFilePath = string.Empty;

    [ObservableProperty]
    private string _selectedPool = string.Empty;

    [ObservableProperty]
    private bool _clearExistingData;

    [ObservableProperty]
    private int _selectedFormatIndex;

    public ObservableCollection<string> Pools { get; } = [];

    public ImportPrizeViewModel(MainConfigHandler configHandler, ILogger<ImportPrizeViewModel>? logger = null)
    {
        _configHandler = configHandler;
        _logger = logger;
        LoadPools();
    }

    private void LoadPools()
    {
        try
        {
            // TODO: 从服务加载奖池列表
            _logger?.LogInformation("奖池列表已加载");
        }
        catch (System.Exception ex)
        {
            _logger?.LogError(ex, "加载奖池列表失败");
        }
    }

    [RelayCommand]
    private void Browse()
    {
        // TODO: 打开文件选择对话框
        _logger?.LogInformation("浏览文件");
    }

    [RelayCommand]
    private void Import()
    {
        if (string.IsNullOrEmpty(SelectedPool))
        {
            _logger?.LogWarning("请先选择奖池");
            return;
        }

        if (string.IsNullOrEmpty(SelectedFilePath))
        {
            _logger?.LogWarning("请选择要导入的文件");
            return;
        }

        try
        {
            // TODO: 执行导入操作
            _logger?.LogInformation("开始导入奖品名单");
        }
        catch (System.Exception ex)
        {
            _logger?.LogError(ex, "导入奖品名单失败");
        }
    }
}
