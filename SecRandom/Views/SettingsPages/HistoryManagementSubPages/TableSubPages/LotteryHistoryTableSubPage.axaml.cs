using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SecRandom.Core.Attributes;

namespace SecRandom.Views.SettingsPages.HistoryManagementSubPages.TableSubPages;

[PageInfo("settings.history.lotteryTable", "\uE8F1", "settings.history")]
public partial class LotteryHistoryTableSubPage : UserControl
{
    public LotteryHistoryTableSubPage()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
