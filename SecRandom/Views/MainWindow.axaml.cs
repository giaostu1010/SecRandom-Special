using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using FluentAvalonia.UI.Windowing;
using SecRandom.Helpers;

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

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        var isMicaSupported = OperatingSystem.IsWindows() 
                              && Environment.OSVersion.Version >= new Version(10, 0, 22000, 0)
                              && AvaloniaUnsafeAccessorHelpers.GetActiveWin32CompositionMode() == AvaloniaUnsafeAccessorHelpers.Win32CompositionMode.WinUiComposition;
        if (isMicaSupported)
        {
            TransparencyLevelHint = [WindowTransparencyLevel.Mica];
            Background = Brushes.Transparent;
        }
    }
}