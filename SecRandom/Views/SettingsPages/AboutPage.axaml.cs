using Avalonia.Controls;
using SecRandom.Core.Attributes;
using SecRandom.Core.Enums;

namespace SecRandom.Views.SettingsPages;

[PageInfo("settings.about", "\ue9e3", PageLocation.Bottom)]
public partial class AboutPage : UserControl
{
    public AboutPage()
    {
        InitializeComponent();
    }
}