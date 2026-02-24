using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using FluentAvalonia.UI.Windowing;

namespace SecRandom.Views;

public partial class MainWindow : AppWindow
{
    public MainWindow()
    {
        InitializeComponent();

        TitleBar.Height = 48;
        TitleBar.ExtendsContentIntoTitleBar = true;
        TitleBar.TitleBarHitTestType = TitleBarHitTestType.Complex;
    }
    
    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        if (App.IsMicaSupported)
        {
            TransparencyLevelHint = [WindowTransparencyLevel.Mica];
            Background = Brushes.Transparent;
        }
    }
}