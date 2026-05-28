using Novellus.Lib.Backend.FileSystems;
using Novellus.Lib.Backend.Logging;
using Novellus.Lib.Core;
using Novellus.Lib.Core.Packages;
using Novellus.Lib.Backend;

namespace Novellus.Lib.Backend.Mergers;

public static class AwbMerger
{
    private sealed record AwbFileMergeInfo(string AbsPath, string WavId, string NormalizedFilename);

    private static bool AcbExists(string path) => File.Exists(Path.ChangeExtension(path, ".acb"));

    private static bool AwbExists(string path) => File.Exists(Path.ChangeExtension(path, ".awb"));

    public static bool SoundArchiveExists(string path) => AcbExists(path) || AwbExists(path);

    private static void CopyAndUnpackArchive(string acbPath, string ogAcbPath, string extension)
    {
        
        ogAcbPath = Path.ChangeExtension(ogAcbPath, ".acb");
        acbPath = Path.ChangeExtension(acbPath, ".acb");

        string ogAwbPath = Path.ChangeExtension(ogAcbPath, ".awb");
        string awbPath = Path.ChangeExtension(acbPath, ".awb");

        Directory.CreateDirectory(Path.GetDirectoryName(acbPath)!);

        if (AwbExists(ogAwbPath))
        {
            Logger.Info($"Copying over {ogAwbPath} to use as base.");
            File.Copy(ogAwbPath, awbPath, true);
        }
        else if (AwbExists(ogAwbPath = Path.Combine(Path.GetDirectoryName(ogAwbPath)!, $"{Path.GetFileNameWithoutExtension(ogAwbPath)}_streamfiles.awb)")))
        {
            Logger.Info($"Copying over {ogAwbPath} to use as base.");
            awbPath = Path.Combine(Path.GetDirectoryName(acbPath)!, Path.GetFileName(ogAwbPath));
            File.Copy(ogAwbPath, awbPath, true);
        }

        if (AcbExists(ogAcbPath))
        {
            Logger.Info($"copying over {ogAcbPath} to use as base.");
            File.Copy(ogAcbPath, acbPath, true);
            Logger.Info($"Unpacking {acbPath}");
            AWB.RunAcbEditor(acbPath);
        }
        else
        {
            Logger.Info($"Unpacking {awbPath}");
            AWB.RunAwbUnpacker(awbPath, extension);
        }
    }

    private static AwbFileMergeInfo? GetAwbMergeInfo(string file)
    {
        // todo: some validations here
        var fileName = Path.GetFileName(file);
        var fileNameWithoutExt = Path.GetFileNameWithoutExtension(file);
        var fileExt = Path.GetExtension(file);

        if (!fileName.Contains('_'))
        {
            string wavId = fileNameWithoutExt.PadLeft(5, '0');
            return new AwbFileMergeInfo(file, wavId, $"{wavId}{fileExt}");
        }
        else
        {
            string wavId = fileNameWithoutExt[..fileNameWithoutExt.IndexOf('_')].PadLeft(5, '0');
            return new AwbFileMergeInfo(file, wavId, $"{wavId}_streaming{fileExt}");
        }

    }
    
    private static void ProcessFilesIntoMap(
        Dictionary<string, Dictionary<string, AwbFileMergeInfo>> awbsMap,
        string absolutePath,
        string relativeAwbPath,
        string packageId)
    {
        var files = Directory.EnumerateFiles(absolutePath).ToList();
        if (files.Count == 0)
        {
            Logger.Warn($"No files found on directory '{absolutePath}' of '{packageId}' package. Skipping...");
            return;
        }

        if (!awbsMap.TryGetValue(relativeAwbPath, out var wavMap))
        {
            wavMap = new Dictionary<string, AwbFileMergeInfo>(StringComparer.OrdinalIgnoreCase);
            awbsMap[relativeAwbPath] = wavMap;
        }

        foreach (var file in files)
        {
            var info = GetAwbMergeInfo(file);
            if (info is null)
            {
                Logger.Warn($"File '{file}' of '{packageId}' package doesn't follow the expected naming convention.");
                continue;
            }
            wavMap[info.WavId] = info;
        }
    }

    private static void ProcessPackageDirectories(
        Dictionary<string, Dictionary<string, AwbFileMergeInfo>> awbsMap,
        IPackage package,
        string gameId)
    {
        var pkgAwbPath = Path.Combine(package.Path, "AWB");
        if (!Directory.Exists(pkgAwbPath)) return;

        var leafDirs = Directory.EnumerateDirectories(pkgAwbPath, "*", SearchOption.AllDirectories)
            .Where(d => !Directory.EnumerateDirectories(d, "*", SearchOption.TopDirectoryOnly).Any())
            .Select(d => Path.GetRelativePath(pkgAwbPath, d));

        foreach (var relativePath in leafDirs)
        {
            var dumpAcbPath = Path.Combine(Folders.Dumps, gameId, relativePath);
            if (!SoundArchiveExists(dumpAcbPath))
            {
                Logger.Warn($"No sound archive found on unpacked game files '{dumpAcbPath}' " +
                            $"required by '{package.Metadata.Id}' package. Did you forget to unpack the game files?");
                //continue;
            }

            var absolutePath = Path.Combine(pkgAwbPath, relativePath);
            ProcessFilesIntoMap(awbsMap, absolutePath, relativePath, package.Metadata.Id);
        }
    }

    private static void ProcessPackageApiCalls(
        Dictionary<string, Dictionary<string, AwbFileMergeInfo>> awbsMap,
        IPackage package,
    string gameId)
    {
        var awbApiCalls = ApiCalls.ResolveArgs(package, "AddFolderToAwb", ArgKind.PackagePath, ArgKind.Path);
        if (awbApiCalls is null || awbApiCalls.Length == 0) return;

        Logger.Debug("RUN: AddFolderToAwb");
        Logger.Debug("WITH:");
        foreach (var arg in awbApiCalls) Logger.Debug($"\t{arg}");

        foreach (var (absolutePath, relativeAwbPath) in awbApiCalls)
        {            
            var dumpAcbPath = Path.Combine(Folders.Dumps, gameId, relativeAwbPath);
            if (!SoundArchiveExists(dumpAcbPath))
            {
                Logger.Warn($"No sound archive found on unpacked game files '{dumpAcbPath}' " +
                            $"required by '{package.Metadata.Id}' package. Did you forget to unpack the game files?");
                continue;
            }

            ProcessFilesIntoMap(awbsMap, absolutePath, relativeAwbPath, package.Metadata.Id);
        }
    }

    public static void Merge(IEnumerable<IPackage> packages, string gameId, string outputPath)
    {
        Logger.Debug("Started to merge awbs");
        // relative to vanilla -> (wavid -> awb merge info)
        Dictionary<string, Dictionary<string, AwbFileMergeInfo>> awbsMap = new(StringComparer.OrdinalIgnoreCase);

        // generate the map of awb files to merge based on package directories and api calls
        // the two functions are separate for better readability
        foreach (var package in packages)
        {
            Logger.Debug($"Processing package: {package.Metadata.Id}");
            ProcessPackageDirectories(awbsMap, package, gameId);
            ProcessPackageApiCalls(awbsMap, package, gameId);
        }

        Parallel.ForEach(awbsMap, kvp =>
        {
            string dumpAcbPath = Path.Combine(Folders.Dumps, gameId, kvp.Key);
            string outputAcbPath = Path.Combine(outputPath, kvp.Key);
            CopyAndUnpackArchive(outputAcbPath, dumpAcbPath, ".adx");
            
            foreach (var file in kvp.Value.Values)
            {
                File.Copy(file.AbsPath, Path.Combine(outputAcbPath, file.NormalizedFilename), true);
            }

            Logger.Info($"Repacking {outputAcbPath}");
            if (AcbExists(outputAcbPath))
                AWB.RunAcbEditor(outputAcbPath);
            else
                AWB.RunAwbRepacker(outputAcbPath);
            Directory.Delete(outputAcbPath, true);
        });
    }
}