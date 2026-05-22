using Novellus.Lib.Backend;
using Novellus.Lib.Backend.FileSystems;
using Novellus.Lib.Backend.Logging;
using Novellus.Lib.Backend.Mergers;
using Novellus.Lib.Core.Packages;
using Novellus.Lib.Core.Plugins;

namespace Novellus.Lib.Managers.PQ2;

internal static class PQ2Info
{
    private const string ID = "pq2";
    private const string NAME = "Persona Q2";
    internal static readonly GameInfo GameInfo = new GameInfo(ID, NAME);
}

public sealed class PQ2Integration : IGameIntegration
{
    public GameInfo Game => PQ2Info.GameInfo;
    public Type ConfigType => typeof(ConfigPQ2);
    public Type ManagerType =>  typeof(PQ2Manager);
}

public sealed class PQ2Manager(ConfigPQ2 config) : ModManager(PQ2Info.GameInfo), ILaunchable
{
    public override Task Build(IEnumerable<IPackage> sortedPackages)
    {
        AwbMerger.Merge(sortedPackages, GameInfo.Identifier, config.OutputPath);
        throw new NotImplementedException();
    }

    public override async Task Unpack() // this is the same as PQ1's unpacking process, consider move to a shared function
    {
        if (!File.Exists(config.DataCpkPath))
        {
            Logger.Error($"Couldn't find {config.DataCpkPath}. Please correct the file path.");
            return;
        }

        await Task.Run(() =>
        {
            var dataFiles = FilteredCpkCsv.Get("filtered_data_pq2.csv");
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
