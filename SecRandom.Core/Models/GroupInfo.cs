namespace SecRandom.Core.Models;

public class GroupInfo
{
    public string Name { get; set; }
    public string Id { get; }
    public string IconGlyph { get; }
    
    public GroupInfo(string name, string id, string iconGlyph)
    {
        Name = name;
        Id = id;
        IconGlyph = iconGlyph;
    }
}