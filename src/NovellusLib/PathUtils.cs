namespace NovellusLib;

internal static class PathUtils
{
    // extracted from reloaded ii mod installer lib
    public static void TryDeleteDirectory(string path, bool recursive = true)
    {
        try { Directory.Delete(path, recursive); }
        catch (Exception) { /* Ignored */ }
    }
}
