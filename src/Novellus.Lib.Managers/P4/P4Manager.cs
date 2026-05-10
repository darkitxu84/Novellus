using Novellus.Lib.Backend;
using Novellus.Lib.Backend.FileSystems;
using Novellus.Lib.Backend.Logging;
using Novellus.Lib.Backend.Packages;
using Novellus.Lib.Core.Packages;
using Novellus.Lib.Core.Plugins;

namespace Novellus.Lib.Managers.P4;

internal static class P4Info
{
    private const string ID = "p4";
    private const string NAME = "Persona 4";
    internal static readonly GameInfo GameInfo = new GameInfo(ID, NAME);
}

internal sealed class P4Integration : IGameIntegration
{
    public GameInfo Game => P4Info.GameInfo;
    public Type ConfigType => typeof(ConfigP4);
    public Type ManagerType => typeof(P4Manager);
}

public sealed class P4Manager(ConfigP4 config) : ModManager(P4Info.GameInfo), ILaunchable
{
    public override Task Build(IEnumerable<IPackage> sortedPackages)
    {
        throw new NotImplementedException();
    }
    // this is actually the same as P3FES, consider some refactoring later like move to a base class or smth
    public override async Task Unpack()
    {
        if (!File.Exists(config.ISOPath))
        {
            Logger.Error($"ISO file not found at specified path: {config.ISOPath}.");
            return;
        }

        const string filesFilter = "*.BIN *.PAK *.PAC *.TBL *.SPR *.BF *.BMD *.PM1 *.bf *.bmd *.pm1 *.FPC -r";

        string btlCvmPath = Path.Combine(PathToUnpack, "BTL.CVM");
        string dataCvmPath = Path.Combine(PathToUnpack, "DATA.CVM");

        await Task.Run(() =>
        {
            PathUtils.TryCreateDirectory(PathToUnpack);

            ZipFile.Extract(config.ISOPath, PathToUnpack, filter: "BTL.CVM DATA.CVM");
            ZipFile.Extract(btlCvmPath, Path.Combine(PathToUnpack, "BTL"), filesFilter);
            ZipFile.Extract(dataCvmPath, Path.Combine(PathToUnpack, "DATA"), filesFilter);

            File.Delete($@"{PathToUnpack}\BTL.CVM");
            File.Delete($@"{PathToUnpack}\DATA.CVM");

            Logger.Info("Unpacking extracted files");
            PAK.ExtractWantedFiles(PathToUnpack);
        });
        Logger.Info($"({GameInfo.Name}): Finished unpacking base files!");
    }

    public void Launch()
    {
        throw new NotImplementedException();
    }
}
