using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.VisualTree;
using FluentAvalonia.UI.Controls;
using SecRandom.Core.Abstraction;
using SecRandom.Models.Config;
using SecRandom.ViewModels;

namespace SecRandom.Views;

public partial class FloatingWindow : Window
{
    public ViewModelBase ViewModel { get; } = IAppHost.GetService<ViewModelBase>();
    public bool CanClose { get; set; } = false;
    
    public FloatingWindow()
    {
        DataContext = this;
        InitializeComponent();
        
        RenderOptions.SetTextRenderingMode(this, TextRenderingMode.Antialias);
        RenderOptions.SetBitmapInterpolationMode(this, BitmapInterpolationMode.HighQuality);
        RenderOptions.SetEdgeMode(this, EdgeMode.Antialias);
        
        Closing += OnClosing;
        AddHandler(PointerPressedEvent, OnPointerPressed, handledEventsToo: true);
        AddHandler(PointerReleasedEvent, OnPointerReleased, handledEventsToo: true);

        ViewModel.Config.FloatingWindowSettings.PropertyChanged += (sender, args) =>
        {
            CheckIsVisibleValidate();
            OnLoaded(this, new RoutedEventArgs());
        };
    }

    private void CheckIsVisibleValidate()
    {
        var settings = ViewModel.Config.FloatingWindowSettings;
        if (!settings.IsRollCallButtonEnabled &&
            !settings.IsQuickDrawButtonEnabled &&
            !settings.IsLotteryButtonEnabled &&
            !settings.IsFaceDrawButtonEnabled &&
            !settings.IsTimerButtonEnabled)
        {
            settings.IsRollCallButtonEnabled = true;
        }
    }

    private void OnClosing(object? sender, WindowClosingEventArgs e)
    {
        if (!CanClose)
        {
            e.Cancel = true;
        }
    }
    
    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        Position = new PixelPoint(ViewModel.Config.FloatPosition.X, ViewModel.Config.FloatPosition.Y);
        if (App.IsAcrylicBlurSupported && ViewModel.Config.FloatingWindowSettings.IsAcrylicBackgroundEnabled)
        {
            TransparencyLevelHint = [WindowTransparencyLevel.AcrylicBlur];
        }
        else
        {
            TransparencyLevelHint = [WindowTransparencyLevel.Transparent];
        }
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            var source = e.Source as Control;
        
            if (IsChildOfButton(source))
            {
                return;
            }
            
            BeginMoveDrag(e);
        }
    }
    
    private bool IsChildOfButton(Visual? visual)
    {
        while (visual != null)
        {
            if (visual is Button or CommandBarButton)
                return true;
            visual = visual.GetVisualParent();
        }
        return false;
    }
    
    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        
        ViewModel.Config.FloatPosition = new FloatPositionConfig { X = Position.X, Y = Position.Y };
    }

    private void OpenMainWindowButton_OnClick(object? sender, RoutedEventArgs e)
    {
        App.ShowMainWindow();
    }
}
