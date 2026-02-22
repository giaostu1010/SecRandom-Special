namespace SecRandom.Core;

public static partial class Utils
{
    public static string GetFilePath(params string[] strings)
    {
        var path = Path.Combine([AppContext.BaseDirectory, "data", ..strings]);
        
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        
        return path;
    }
}