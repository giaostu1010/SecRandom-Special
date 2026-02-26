using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SecRandom.Services.Config;

namespace SecRandom.ViewModels.ListManagementSubPages.TablePreview;

public partial class LotteryTablePreviewViewModel : ObservableObject
{
    private readonly MainConfigHandler _configHandler;
    private readonly ILogger<LotteryTablePreviewViewModel>? _logger;

    [ObservableProperty]
    private string _selectedPool = string.Empty;

    [ObservableProperty]
    private int _totalPrizes;

    public ObservableCollection<string> Pools { get; } = [];

    public ObservableCollection<PrizeTableRow> Prizes { get; } = [];

    public LotteryTablePreviewViewModel(MainConfigHandler configHandler, ILogger<LotteryTablePreviewViewModel>? logger = null)
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

    partial void OnSelectedPoolChanged(string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            LoadPrizes(value);
        }
    }

    private void LoadPrizes(string poolName)
    {
        try
        {
            Prizes.Clear();
            // TODO: 从服务加载奖品列表
            TotalPrizes = Prizes.Count;
            _logger?.LogInformation("奖品列表已加载: {Pool}", poolName);
        }
        catch (System.Exception ex)
        {
            _logger?.LogError(ex, "加载奖品列表失败");
        }
    }

    [RelayCommand]
    private void Refresh()
    {
        if (!string.IsNullOrEmpty(SelectedPool))
        {
            LoadPrizes(SelectedPool);
        }
    }
}

public partial class PrizeTableRow : ObservableObject
{
    [ObservableProperty]
    private int _serial;

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
