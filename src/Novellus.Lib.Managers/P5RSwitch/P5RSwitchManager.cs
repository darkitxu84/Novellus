using Novellus.Lib.Backend;
using Novellus.Lib.Backend.FileSystems;
using Novellus.Lib.Backend.Logging;
using Novellus.Lib.Core.Packages;
using Novellus.Lib.Core.Plugins;

namespace Novellus.Lib.Managers.P5RSwitch;

internal static class P5RSwitchInfo
{
    private const string ID = "p5rswitch";
    private const string NAME = "Persona 5 Royale (Switch)";
    internal static readonly GameInfo GameInfo = new GameInfo(ID, NAME);
}

internal sealed class P5RSwitchSupport : IGameSupport
{
    public GameInfo Game => P5RSwitchInfo.GameInfo;
    public Type ConfigType => typeof(ConfigP5RSwitch);
    public Type ManagerType => typeof(P5RSwitchManager);
}

public sealed class P5RSwitchManager(ConfigP5RSwitch config) : ModManager(P5RSwitchInfo.GameInfo), ILaunchable
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
    public override Task Build(IEnumerable<IPackage> sortedPackages)
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
            PathUtils.TryCreateDirectory(PathToUnpack);

            Logger.Info($"Extracting PATCH1.CPK");
            CriCPK.Unpack(patch1Path, PathToUnpack);

            Logger.Info($"Extracting ALL_USEU.CPK (This will take awhile)");
            CriCPK.Unpack(allUseuPath, PathToUnpack);

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
