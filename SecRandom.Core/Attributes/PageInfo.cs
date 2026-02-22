using FluentAvalonia.UI.Controls;
using SecRandom.Core.Controls;
using SecRandom.Core.Enums;

namespace SecRandom.Core.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class PageInfo : Attribute
{
    public bool IsSeparator { get; }
    
    public string Name { get; }
    public string Id { get; }
    public string IconGlyph { get; }
    
    public PageLocation Location { get; }
    public bool UseFullWidth { get; }
    public bool HidePageTitle { get; }

    public PageInfo(bool isSeparator, PageLocation location = PageLocation.Top)
    {
        if (isSeparator)
        {
            IsSeparator = true;
            
            Name = "分割线";
            Id = "separator";
            IconGlyph = "";

            Location = location;
            UseFullWidth = false;
            HidePageTitle = false;
        }
        else
        {
            throw new ArgumentException("isSeparator 为 false!!!!!");
        }
    }
    
    public PageInfo(string name, string id, string iconGlyph = "\uE06F", PageLocation location = PageLocation.Top, bool useFullWidth = false, bool hidePageTitle = false)
    {
        IsSeparator = false;
        
        Name = name;
        Id = id;
        IconGlyph = iconGlyph;

        Location = location;
        UseFullWidth = useFullWidth;
        HidePageTitle = hidePageTitle;
    }

    public NavigationViewItemBase ToNavigationViewItemBase()
    {
        if (IsSeparator)
        {
            return new NavigationViewItemSeparator();
        }

        return new NavigationViewItem
        {
            IconSource = new FluentIconSource(IconGlyph),
            Content = Name,
            Tag = this
        };
    }
}