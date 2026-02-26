using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SecRandom.Services.Config;

namespace SecRandom.ViewModels.ListManagementSubPages.LotterySubPages;

public partial class ExportPrizeViewModel : ObservableObject
{
    private readonly MainConfigHandler _configHandler;
    private readonly ILogger<ExportPrizeViewModel>? _logger;

    [ObservableProperty]
    private string _savePath = string.Empty;

    [ObservableProperty]
    private int _selectedFormatIndex;

    [ObservableProperty]
    private string _selectedPool = string.Empty;

    public ObservableCollection<string> Pools { get; } = new();

    public ExportPrizeViewModel(MainConfigHandler configHandler, ILogger<ExportPrizeViewModel>? logger = null)
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
        // TODO: 打开保存文件对话框
        _logger?.LogInformation("浏览保存位置");
    }

    [RelayCommand]
    private void Export()
    {
        if (string.IsNullOrEmpty(SelectedPool))
        {
            _logger?.LogWarning("请先选择奖池");
            return;
        }

        if (string.IsNullOrEmpty(SavePath))
        {
            _logger?.LogWarning("请选择保存位置");
            return;
        }

        try
        {
            // TODO: 执行导出操作
            _logger?.LogInformation("开始导出奖品名单");
        }
        catch (System.Exception ex)
        {
            _logger?.LogError(ex, "导出奖品名单失败");
        }
    }
}
