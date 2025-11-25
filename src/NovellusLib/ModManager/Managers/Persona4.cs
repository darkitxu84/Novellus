using NovellusLib.Configuration.GameConfigs;
using NovellusLib.FileSystems;
using NovellusLib.Logging;

namespace NovellusLib.ModManager.Managers;

public class P4ModManager(ConfigP4 config) : ModManager(Game.P4), ILaunchable
{
    public override Task Build()
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
            TryCreateUnpackDirectory();

            ZipFile.Extract(config.ISOPath, PathToUnpack, filter: "BTL.CVM DATA.CVM");
            ZipFile.Extract(btlCvmPath, Path.Combine(PathToUnpack, "BTL"), filesFilter);
            ZipFile.Extract(dataCvmPath, Path.Combine(PathToUnpack, "DATA"), filesFilter);

            File.Delete($@"{PathToUnpack}\BTL.CVM");
            File.Delete($@"{PathToUnpack}\DATA.CVM");

            Logger.Info("Unpacking extracted files");
            PAK.ExtractWantedFiles(PathToUnpack);
        });
        Logger.Info($"({Game.P4.Name()}): Finished unpacking base files!");
    }

    public void Launch()
    {
        throw new NotImplementedException();
    }
}
