using Novellus.Lib.Backend.Packages;

namespace Novellus.Lib.Backend;

public static class GameFileService
{
    private static Func<string, string>? _loader;

    public static void Register(Func<string, string> loader)
    {
        _loader = loader;
    }

    public static string GetFile(string relativePath)
    {
        if (_loader is null)
            throw new InvalidOperationException("No file loader registered.");
        return _loader(relativePath);
    }

    public static bool CopyIfNotExist(string relativePath, string outputPath)
    {
        var normalizedRelPath = PathUtils.Normalize(relativePath);
        var fileToCopy = PathUtils.GetPhysicalFile(normalizedRelPath);
        var outputPhysicalFile = Path.Combine(outputPath, fileToCopy);

        if (!File.Exists(outputPhysicalFile))
        {
            var originalFile = GetFile(fileToCopy);
            if (string.IsNullOrEmpty(originalFile)) return false;

            string directoryName = Path.GetDirectoryName(outputPhysicalFile)!;
            Directory.CreateDirectory(directoryName);
            File.Copy(originalFile, outputPhysicalFile, true);
        }
        return true;
    }
}