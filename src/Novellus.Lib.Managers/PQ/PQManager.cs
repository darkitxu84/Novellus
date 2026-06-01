using Novellus.Lib.Backend;
using Novellus.Lib.Backend.Build.BinaryPatching;
using Novellus.Lib.Backend.FileSystems;
using Novellus.Lib.Backend.Logging;
using Novellus.Lib.Backend.Mergers;
using Novellus.Lib.Core.Packages;
using Novellus.Lib.Core.Plugins;

namespace Novellus.Lib.Managers.PQ;

internal static class PQInfo
{
    private const string ID = "pq";
    private const string NAME = "Persona Q";
    internal static readonly GameInfo GameInfo = new GameInfo(ID, NAME);
}

public sealed class PQIntegration : IGameIntegration
{
    public GameInfo Game => PQInfo.GameInfo;
    public Type ConfigType => typeof(ConfigPQ);
    public Type ManagerType =>  typeof(PQManager);
}

public sealed class PQManager(ConfigPQ config) : ModManager(PQInfo.GameInfo), ILaunchable
{
    public override async Task Build(IEnumerable<IPackage> sortedPackages)
    {
        await Task.Run(() =>
        {
            GameFileService.Register(GetFileFromGame);
            AwbMerger.Merge(sortedPackages, config.OutputPath);
            PACMerger.Merge(sortedPackages, config.OutputPath);
            BinaryPatcher.Patch(sortedPackages, config.OutputPath);
        });
        throw new NotImplementedException();
    }

    public override async Task Unpack()
    {
        if (!File.Exists(config.DataCpkPath))
        {
            Logger.Error($"Couldn't find {config.DataCpkPath}. Please correct the file path.");
            return;
        }

        await Task.Run(() =>
        {
            var dataFiles = FilteredCpkCsv.Get("filtered_data_pq.csv");
            if (dataFiles is null)
                return;

            PathUtils.TryCreateDirectory(PathToUnpack);

            Logger.Info($"Extracting data.cpk");
            CriCPK.Unpack(config.DataCpkPath, PathToUnpack, dataFiles);

            Logger.Info("Unpacking extracted files");
            PAK.ExtractWantedFiles(PathToUnpack);
        });
        Logger.Info($"({GameInfo.Name}); Finished unpacking base files!");
    }
    public void Launch()
    {
        throw new NotImplementedException();
    }
}
