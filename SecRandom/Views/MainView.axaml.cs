using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using DynamicData;
using FluentAvalonia.UI.Controls;
using Microsoft.Extensions.DependencyInjection;
using SecRandom.Core.Abstraction;
using SecRandom.Core.Attributes;
using SecRandom.Core.Controls;
using SecRandom.Core.Enums;
using SecRandom.Core.Services;
using SecRandom.Services.Config;
using SecRandom.ViewModels;

namespace SecRandom.Views;

public partial class MainView : UserControl, INavigationPageFactory
{
    public MainViewModel ViewModel { get; } = IAppHost.GetService<MainViewModel>();
    private const string DefaultMainPageId = "main.rollCall";
    
    private AppToastAdorner? _appToastAdorner;
    private bool _isAdornerAdded;
    
    public MainView()
    {
        DataContext = this;
        InitializeComponent();

        NavigationFrame.NavigationPageFactory = this;
        BuildNavigationMenuItems();
        SelectNavigationItemById(DefaultMainPageId);
        
        RenderOptions.SetTextRenderingMode(this, TextRenderingMode.Antialias);
        RenderOptions.SetBitmapInterpolationMode(this, BitmapInterpolationMode.HighQuality);
        RenderOptions.SetEdgeMode(this, EdgeMode.Antialias);
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        if (ViewModel.SelectedPageInfo is null)
        {
            SelectNavigationItemById(DefaultMainPageId);
        }
        
        if (Content is not Control element || _isAdornerAdded)
        {
            return;
        }

        var layer = AdornerLayer.GetAdornerLayer(element);
        var appToastAdorner = _appToastAdorner = new AppToastAdorner(this);
        layer?.Children.Add(appToastAdorner);
        AdornerLayer.SetAdornedElement(appToastAdorner, this);
        _isAdornerAdded = true;
    }
    
    private void OnUnloaded(object? sender, RoutedEventArgs e)
    {
        DataContext = null;
    }

    private void BuildNavigationMenuItems()
    {
        ViewModel.NavigationViewItems.Clear();
        ViewModel.NavigationViewFooterItems.Clear();
        
        ViewModel.NavigationViewItems
            .AddRange(PagesRegistryService.MainItems
                .Where(info => info.Location == PageLocation.Top)
                .Select(info => info.ToNavigationViewItemBase()));
        
        ViewModel.NavigationViewFooterItems
            .AddRange(PagesRegistryService.MainItems
                .Where(info => info.Location == PageLocation.Bottom)
                .Select(info => info.ToNavigationViewItemBase()));
        
        ViewModel.NavigationViewFooterItems.Add(
            new PageInfo(true, PageLocation.Bottom).ToNavigationViewItemBase());
        ViewModel.NavigationViewFooterItems.Add(
            new PageInfo(Langs.Resources.Settings, "settings", "\uEF27", PageLocation.Bottom).ToNavigationViewItemBase());
    }

    public void SelectNavigationItemById(string id)
    {
        var info = PagesRegistryService.MainItems.FirstOrDefault(info => info.Id == id);
        
        if (info != null)
        {
            CoreNavigate(info);
        }
    }
    
    private void SelectNavigationItem(PageInfo info)
    {
        var item = ViewModel.NavigationViewItems.FirstOrDefault(item => Equals(item.Tag, info)) ??
                   ViewModel.NavigationViewFooterItems.FirstOrDefault(item => Equals(item.Tag, info));
        ViewModel.SelectedNavigationViewItem = item;
    }

    private void CoreNavigate(PageInfo info)
    {
        if (info.Id == "settings")
        {
            App.ShowSettingsWindow();
            return;
        }
        
        ViewModel.FrameContent = null;
        SelectNavigationItem(info);
        ViewModel.SelectedPageInfo = info;
        NavigationFrame.NavigateFromObject(info);
    }
    
    private void NavigationView_OnItemInvoked(object? sender, NavigationViewItemInvokedEventArgs e)
    {
        if (e.InvokedItemContainer is NavigationViewItem { Tag: PageInfo info })
        {
            CoreNavigate(info);
        }
    }

    private void TogglePaneButton_OnClick(object? sender, RoutedEventArgs e)
    {
        NavigationView.IsPaneOpen = !NavigationView.IsPaneOpen;
    }

    public Control? GetPage(Type srcType)
    {
        return Activator.CreateInstance(srcType) as Control;
    }

    public Control? GetPageFromObject(object target)
    {
        if (target is not PageInfo info)
        {
            return null;
        }
        
        return IAppHost.Host!.Services.GetKeyedService<UserControl>(info.Id);
    }
}
