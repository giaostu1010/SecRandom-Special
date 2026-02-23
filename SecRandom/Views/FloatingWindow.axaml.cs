using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace SecRandom.Views;

public partial class FloatingWindow : Window
{
    public FloatingWindow()
    {
        InitializeComponent();
        
        RenderOptions.SetTextRenderingMode(this, TextRenderingMode.Antialias);
        RenderOptions.SetBitmapInterpolationMode(this, BitmapInterpolationMode.HighQuality);
        RenderOptions.SetEdgeMode(this, EdgeMode.Antialias);
    }

    private void OpenMainWindowButton_OnClick(object? sender, RoutedEventArgs e)
    {
        App.ShowMainWindow();
    }

    private void Control_OnLoaded(object? sender, RoutedEventArgs e)
    {
        var bounds = RootStackPanel.Bounds;
        Height = bounds.Height + 24 * 2;
        Width = bounds.Width + 24 * 2;
    }
}
