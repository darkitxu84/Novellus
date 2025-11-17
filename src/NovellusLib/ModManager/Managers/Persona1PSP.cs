using NovellusLib.Configuration.GameConfigs;
using NovellusLib.FileSystems;

namespace NovellusLib.ModManager.Managers;

public class P1PSPModManager(ConfigP1PSP config) : ModManager(Game.P1PSP), ILaunchable
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

        string ebootPath = Path.Combine(PathToUnpack, "PSP_GAME", "SYSDIR");

        await Task.Run(() =>
        {
            ZipFile.Extract(config.ISOPath, PathToUnpack);
            File.Move($@"{ebootPath}\EBOOT.BIN", $@"{ebootPath}\EBOOT_ENC.BIN");
            PSPElf.Decrypt($@"{ebootPath}\EBOOT_ENC.BIN", $@"{ebootPath}\EBOOT.BIN");
            File.Delete($@"{ebootPath}\EBOOT_ENC.BIN");
        });
        Logger.Info("[INFO] Finished unpacking base files!");
    }
    public void Launch()
    {
        throw new NotImplementedException();
    }
}
