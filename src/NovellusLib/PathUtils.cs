using NovellusLib.Logging;

namespace NovellusLib;

internal static class PathUtils
{
    // extracted from reloaded ii mod installer lib
    public static void TryDeleteDirectory(string path, bool recursive = true)
    {
        try { Directory.Delete(path, recursive); }
        catch (Exception) { /* Ignored */ }
    }

    public static void TryCreateDirectory(string path)
    {
        try { Directory.CreateDirectory(path); }
        catch (Exception ex) { Logger.Error($"Unable to create directory {path}: {ex.Message}"); }
    }
}
