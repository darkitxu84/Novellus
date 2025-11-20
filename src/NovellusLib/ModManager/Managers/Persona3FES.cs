using NovellusLib.Configuration.GameConfigs;
using NovellusLib.FileSystems;

namespace NovellusLib.ModManager.Managers;

public class P3FESModManager(ConfigP3F config) : ModManager(Game.P3FES), ILaunchable
{
    public override Task Build()
    {
        throw new NotImplementedException();
    }

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
            ZipFile.Extract(config.ISOPath, PathToUnpack, filter: "BTL.CVM DATA.CVM");
            ZipFile.Extract(btlCvmPath, Path.Combine(PathToUnpack, "BTL"), filesFilter);
            ZipFile.Extract(dataCvmPath, Path.Combine(PathToUnpack, "DATA"), filesFilter);

            File.Delete($@"{PathToUnpack}\BTL.CVM");
            File.Delete($@"{PathToUnpack}\DATA.CVM");

            Logger.Info("Unpacking extracted files");
            PAK.ExtractWantedFiles(PathToUnpack);
        });
        Logger.Info($"{Game.P3FES.Name()}: Finished unpacking base files!");
    }

    public void Launch()
    {
        throw new NotImplementedException();
    }
}
