using Avalonia.Controls;
using SecRandom.Core.Attributes;
using SecRandom.Core.Enums;

namespace SecRandom.Views.SettingsPages;

[PageInfo("关于", "settings.about", "\uE9E4", PageLocation.Bottom)]
public partial class AboutPage : UserControl
{
    public AboutPage()
    {
        InitializeComponent();
    }
}