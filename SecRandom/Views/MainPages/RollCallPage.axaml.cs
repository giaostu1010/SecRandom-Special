using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SecRandom.Core.Attributes;

namespace SecRandom.Views.MainPages;

[PageInfo("main.rollCall", "\uecaa")]
public partial class RollCallPage : UserControl
{
    public RollCallPage()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}