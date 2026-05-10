using Novellus.Lib.Backend;
using Novellus.Lib.Backend.FileSystems;
using Novellus.Lib.Backend.Logging;
using Novellus.Lib.Backend.Packages;
using Novellus.Lib.Core.Packages;
using Novellus.Lib.Core.Plugins;
using Novellus.Lib.Managers.P4G32;

namespace Novellus.Lib.Managers.P4GVita;

internal static class P4GVitaInfo
{
    private const string ID = "p4gvita";
    private const string NAME = "Persona 4 Golden (Vita)";
    internal static readonly GameInfo GameInfo = new GameInfo(ID, NAME);
}

internal sealed class P4GVitaSupport : IGameSupport
{
    public GameInfo Game => P4GVitaInfo.GameInfo;
    public Type ConfigType => typeof(ConfigP4GVita);
    public Type ManagerType => typeof(P4GVitaManager);
}

public sealed class P4GVitaManager(ConfigP4GVita config) : ModManager(P4GVitaInfo.GameInfo)
{
    public override Task Build(IEnumerable<IPackage> sortedPackages)
    {
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
            var dataFiles = FilteredCpkCsv.Get("filtered_p4gdata");
            if (dataFiles is null)
                return;

            PathUtils.TryCreateDirectory(PathToUnpack);

            Logger.Info($"Extracting data.cpk");
            CriCPK.Unpack(config.DataCpkPath, PathToUnpack, dataFiles);

            Logger.Info("Unpacking extracted files");
            PAK.ExtractWantedFiles(PathToUnpack);
        });
        Logger.Info($"({GameInfo.Name}): Finished unpacking base files!");
    }
}
