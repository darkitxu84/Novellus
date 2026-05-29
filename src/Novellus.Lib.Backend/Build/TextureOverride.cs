using Novellus.Lib.Backend.Logging;
using Novellus.Lib.Core.Packages;
using System.Runtime.InteropServices;

namespace Novellus.Lib.Backend.Build;

public static class TextureOverride
{
    private static void CopyTexture(string texturePath, string outputPath)
    {
        string dest = Path.Combine(outputPath, Path.GetFileName(texturePath));
        if (File.Exists(dest))
        {
            Logger.Error($"Cannot copy {texturePath} to {outputPath}: file already exists!");
            return;
        }

        // use symlinks on linux
        if (OperatingSystem.IsLinux())
        {
            File.CreateSymbolicLink(dest, texturePath);
            return;
        }

        if (OperatingSystem.IsWindows())
        {
            // use hardlinks in windows if the outputpath and the texture are in the same drive
            if (string.Equals(Path.GetPathRoot(texturePath), Path.GetPathRoot(outputPath), StringComparison.OrdinalIgnoreCase))
            {
                if (!Symlinks.CreateHardLinkA(dest, texturePath, IntPtr.Zero)) 
                    Logger.Error($"Cannot create hardlink for {texturePath}:");
                return;
            }

            File.Copy(texturePath, dest);
        }
    }

    public static void Process(IEnumerable<IPackage> packages, string outputPath)
    {
        if (Directory.Exists(outputPath))
        {
            foreach (var file in Directory.EnumerateFiles(outputPath, "*", SearchOption.AllDirectories))
                File.Delete(file);
        }
        else
        {
            Directory.CreateDirectory(outputPath);
        }

        Dictionary<string, string> filesToCopy = new(StringComparer.OrdinalIgnoreCase);

        foreach (var package in packages)
        { 
            var texturesDir = Path.Combine(package.Path, "texture_override");
            var include = ApiCalls.ResolveArgs(package, "AddTextureOverride", ArgKind.PackageFile);

            var texturesFiles = Directory.Exists(texturesDir)
                ? Directory.EnumerateFiles(texturesDir, "*", SearchOption.AllDirectories).ToList()
                : [];
            if (include is not null) texturesFiles.AddRange(include);

            foreach (var file in texturesFiles)
            {
                string fileName = Path.GetFileName(file);
                filesToCopy[fileName] = file;
            }
        }
        foreach (var (textureName, texturePath) in filesToCopy)
        {
            Logger.Info($"Copying {textureName} to {outputPath}");
            CopyTexture(texturePath, outputPath);
        }
    }
}