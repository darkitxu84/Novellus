using Novellus.Lib.Backend.FileSystems;
using Novellus.Lib.Backend.Logging;
using Novellus.Lib.Core.Packages;
using Novellus.Lib.Core.Plugins;

namespace Novellus.Lib.Managers.P3F;

internal static class P3FInfo
{
    private const string ID = "p3fes";
    private const string NAME = "Persona 3 FES";
    internal static readonly GameInfo GameInfo = new GameInfo(ID, NAME);
}

internal sealed class P3FSupport : IGameSupport
{
    public GameInfo Game => P3FInfo.GameInfo;
    public Type ConfigType => typeof(ConfigP3F);
    public Type ManagerType => typeof(P3FManager);
}

public sealed class P3FManager(ConfigP3F config) : ModManager(P3FInfo.GameInfo), ILaunchable
{
    private const string MERGED_CHEATS_FILENAME = "nv_merged_cheats.pnach";

    private void DeleteOutputLeftovers()
    {
        
    }
    
    public override Task Build(IEnumerable<IPackage> sortedPackages)
    {
        throw new NotImplementedException();
    }

    public override async Task Unpack()
    {
        if (!File.Exists(config.ISOPath))
        {
            Logger.Error($"ISO file not found at specified path: {config.ISOPath}.");
            throw new FileNotFoundException($"ISO file not found at specified path: {config.ISOPath}.");
        }

        const string filesFilter = "*.BIN *.PAK *.PAC *.TBL *.SPR *.BF *.BMD *.PM1 *.bf *.bmd *.pm1 *.FPC -r";

        string btlCvmPath = Path.Combine(PathToUnpack, "BTL.CVM");
        string dataCvmPath = Path.Combine(PathToUnpack, "DATA.CVM");

        await Task.Run(() =>
        {
            Directory.CreateDirectory(PathToUnpack);

            Logger.Info("Extracting CVM files from ISO");
            ZipFile.Extract(config.ISOPath, PathToUnpack, filter: "BTL.CVM DATA.CVM");

            Logger.Info("Extracting BTL.CVM and DATA.CVM");
            ZipFile.Extract(btlCvmPath, Path.Combine(PathToUnpack, "BTL"), filesFilter);
            ZipFile.Extract(dataCvmPath, Path.Combine(PathToUnpack, "DATA"), filesFilter);

            File.Delete(btlCvmPath);
            File.Delete(dataCvmPath);

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
