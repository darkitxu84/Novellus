using Novellus.Lib.Backend.FileSystems;
using Novellus.Lib.Backend.Logging;
using Novellus.Lib.Core.Packages;

namespace Novellus.Lib.Backend.Build.BinaryPatching;

public static class BinaryPatcher
{
    private static byte[]? GetBytesFromString(string str)
    {
        string[] stringData = str.Split(' ');
        byte[] data = new byte[stringData.Length];
        for (int i = 0; i < data.Length; i++)
        {
            try
            {
                data[i] = Convert.ToByte(stringData[i], 16);
            }
            catch (Exception ex)
            {
                Logger.Error($"Couldn't parse hex string {stringData[i]} ({ex.Message}), skipping...");
                return null;
            }
        }
        return data;
    }
    public static void Patch(IEnumerable<IPackage> packages, string outputPath)
    {
        Logger.Info("Patching files...");
        foreach (var package in packages)
        {
            var binPatchesDir = Path.Combine(package.Path, "binarypatches");

            var doNotInclude = ApiCalls.ResolveArgs(package, "RemoveBinaryPatch", ArgKind.Path) ?? [];
            var include = ApiCalls.ResolveArgs(package, "AddBinaryPatch", ArgKind.PackageFile);

            var patchesFiles = Directory.Exists(binPatchesDir)
                ? Directory.EnumerateFiles(binPatchesDir, "*.bp", SearchOption.AllDirectories)
                    .Where(c => !doNotInclude.Contains(Path.GetFileName(c)))
                    .ToList()
                : [];
            if (include is not null) patchesFiles.AddRange(include);

            foreach (var patchFile in patchesFiles)
            {
                var patches = Yaml.TryLoad<BinaryPatches>(patchFile);
                if (patches is null) continue;
                if (patches.Version != 2)
                {
                    Logger.Error($"Invalid version for {patchFile}, skipping...");
                    continue;
                }

                foreach (var patch in patches.Patches)
                {
                    if (!GameFileService.CopyIfNotExist(patch.File, outputPath))
                    {
                        Logger.Warn($"{patch.File} not found in output directory or Dumps directory " +
                                    $"(required by {package.Metadata.Id})");
                        continue;
                    }

                    byte[]? data = GetBytesFromString(patch.Data);
                    if (data is null) continue;

                    void binPatch(Stream stream)
                    {
                        // Add null bytes if offset is greater than count
                        if (patch.Offset > stream.Length)
                        {
                            stream.Position = stream.Length;
                            int padding = (int)(patch.Offset - stream.Length);
                            stream.Write(new byte[padding], 0, padding);
                        }
                        else if (patch.Offset + data.Length > stream.Length)
                            stream.SetLength(patch.Offset);

                        stream.Position = patch.Offset;
                        stream.Write(data, 0, data.Length);
                    }

                    PathUtils.ResolvePathAndEdit(outputPath, PathUtils.Normalize(patch.File), binPatch);
                }
            }
        }
    }
}