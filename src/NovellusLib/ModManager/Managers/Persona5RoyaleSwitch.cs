using NovellusLib.Configuration.GameConfigs;
using NovellusLib.FileSystems;

namespace NovellusLib.ModManager.Managers;

public class P5RSwitchModManager(ConfigP5RSwitch config) : ModManager(Game.P5RSwitch), ILaunchable
{
    private bool CheckNeededCpks()
    {
        var cpksNeeded = new List<string>()
        {
            "ALL_USEU.CPK",
            "PATCH1.CPK"
        };
        var cpks = Directory.GetFiles(config.CpksPath, "*.cpk", SearchOption.TopDirectoryOnly);

        if (cpksNeeded.Except(cpks.Select(x => Path.GetFileName(x))).Any())
        {
            Logger.Error($"Not all cpks needed (ALL_USEU.CPK and PATCH1.CPK) are found in top directory of {config.CpksPath}");
            return false;
        }

        return true;
    }
    public override Task Build()
    {
        throw new NotImplementedException();
    }

    public override async Task Unpack()
    {
        if (!Directory.Exists(config.CpksPath))
        {
            Logger.Error($"Couldn't find {config.CpksPath}. Please correct the file path in config.");
            return;
        }
        if (!CheckNeededCpks())
            return;

        string patch1Path = Path.Combine(config.CpksPath, "PATCH1.CPK");
        string allUseuPath = Path.Combine(config.CpksPath, "ALL_USEU.CPK");

        await Task.Run(() =>
        {
            Logger.Info($"Extracting PATCH1.CPK");
            CriCPK.Unpack(patch1Path, PathToUnpack);

            Logger.Info($"Extracting ALL_USEU.CPK (This will take awhile)");
            CriCPK.Unpack(allUseuPath, PathToUnpack);

            PAK.ExtractWantedFiles(PathToUnpack);
        });
        Logger.Info($"{Game.P5RSwitch.Name()}; Finished unpacking base files!");
    }

    public void Launch()
    {
        throw new NotImplementedException();
    }
}
