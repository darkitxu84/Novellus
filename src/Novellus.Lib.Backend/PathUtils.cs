using AtlusFileSystemLibrary;
using AtlusFileSystemLibrary.FileSystems.PAK;
using Novellus.Lib.Backend.FileSystems;
using Novellus.Lib.Backend.Logging;

namespace Novellus.Lib.Backend;

public static class PathUtils
{
    private static bool ResolvePathInsidePak(PAKFileSystem pac, string[] parts, Action<Stream> edit)
    {
        var contents = pac.EnumerateFiles().ToHashSet();
        var root = string.Empty;

        for (int i = 0; i < parts.Length; i++)
        {
            root = Path.Combine(root, parts[i]);
            string normalizedPath = PAK.NormalizePath(contents, root);
            if (!contents.Contains(normalizedPath)) continue;

            string ext = Path.GetExtension(normalizedPath) ?? ".";
            // this is a file or a PAK at the end of the path
            if (!PAK.IsPak(ext) || i == parts.Length - 1)
            {
                // we can't directly modify the stream that pac.OpenFile returns, so we move the contents to a temporal buffer
                // then we modify the buffer and overwrite the original file
                var fileMemoryFs = new MemoryStream();
                using (var fileFs = pac.OpenFile(normalizedPath))
                    fileFs.CopyTo(fileMemoryFs);
                fileMemoryFs.Position = 0;

                edit(fileMemoryFs);
                fileMemoryFs.Position = 0;
                pac.AddFile(normalizedPath, fileMemoryFs, true, ConflictPolicy.Replace);
                return true;
            }

            var partsToSearchInside = parts[(i + 1)..parts.Length];

            if (!PAKFileSystem.TryOpen(pac.OpenFile(normalizedPath), true, out var insidePac)) return false;
            if (!ResolvePathInsidePak(insidePac, partsToSearchInside, edit))
                return false;
            pac.AddFile(normalizedPath, insidePac.Save(), true, ConflictPolicy.Replace);
            return true;
        }

        return false;
    }

    // we need to resolves paths like DATA/FIELD/SOME.PACK/etc/FILE.S
    // consider move this to a more general utils class if we need to resolve paths in other contexts
    public static bool ResolvePathAndEdit(string root, string relativePath, Action<Stream> edit)
    {
        string[] parts = relativePath.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);
        string fullPath = Path.Combine(root, relativePath);

        for (int i = 0; i < parts.Length; i++)
        {
            string nextRoot = Path.Combine(root, parts[i]);
            string ext = Path.GetExtension(nextRoot) ?? ".";

            if (Directory.Exists(nextRoot))
            {
                if (i == parts.Length - 1)
                {
                    Logger.Error("Ended the path but got a directory");
                    return false;
                }
                root = nextRoot;
                continue;
            }

            // if we don't have a directory or file this path is invalid
            if (!File.Exists(nextRoot))
            {
                Logger.Error($"Part '{parts[i]}' in {fullPath} does not exist!");
                return false;
            }

            // this a single file or this is the end of the path and we got a PAK
            if (!PAK.IsPak(ext) || i == parts.Length - 1)
            {
                using var fs = File.Open(nextRoot, FileMode.Open, FileAccess.ReadWrite);
                edit(fs);
                return true;
            }

            // this is a bit hacky but the PAK cannot be saved if we load it from the disc directly, which is rare
            var pakMemoryFs = new MemoryStream();
            using (var fs = File.OpenRead(nextRoot))
                fs.CopyTo(pakMemoryFs);
            pakMemoryFs.Position = 0;

            if (!PAKFileSystem.TryOpen(pakMemoryFs, true, out var pak))
            {
                Logger.Error($"Failed to open pak file: {nextRoot}");
                return false;
            }

            var partsToSearchInside = parts[(i + 1)..parts.Length];
            if (!ResolvePathInsidePak(pak, partsToSearchInside, edit))
            {
                Logger.Error($"Cannot find file {string.Join('/', partsToSearchInside)} inside {nextRoot}");
                return false;
            }
            pak.Save(nextRoot);
            return true;

        }

        Logger.Error($"Failed to resolve path: {fullPath}");
        return false;
    }

    public static string GetPhysicalFile(string path)
    {
        string[] parts = path.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);
        string paths = string.Empty;

        foreach (string part in parts)
        {
            string ext = Path.GetExtension(part);
            paths = Path.Combine(paths, part);

            if (PAK.IsPak(ext)) break;
        }

        return paths;
    }

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

    public static string NormalizePath(string path)
    {
        return path.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
    }
}
