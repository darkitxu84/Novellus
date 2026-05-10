using Novellus.Lib.Backend;
using Novellus.Lib.Backend.FileSystems;
using Novellus.Lib.Backend.Logging;
using Novellus.Lib.Core.Packages;
using Novellus.Lib.Core.Plugins;

namespace Novellus.Lib.Managers.P1PSP;

internal static class P1PSPInfo
{
    private const string ID = "p1psp";
    private const string NAME = "Persona 1 (PSP)";
    internal static readonly GameInfo GameInfo = new GameInfo(ID, NAME);
}

public sealed class P1PSPSupport : IGameSupport
{
    public GameInfo Game => P1PSPInfo.GameInfo;
    public Type ConfigType => typeof(ConfigP1PSP);
    public Type ManagerType =>  typeof(P1PSPManager);
}

public sealed class P1PSPManager(ConfigP1PSP config) : ModManager(P1PSPInfo.GameInfo), ILaunchable
{
    public override Task Build(IEnumerable<IPackage> sortedPackages)
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
            PathUtils.TryCreateDirectory(PathToUnpack);
            ZipFile.Extract(config.ISOPath, PathToUnpack);
            File.Move($@"{ebootPath}\EBOOT.BIN", $@"{ebootPath}\EBOOT_ENC.BIN");
            PSPElf.Decrypt($@"{ebootPath}\EBOOT_ENC.BIN", $@"{ebootPath}\EBOOT.BIN");
            File.Delete($@"{ebootPath}\EBOOT_ENC.BIN");
        });
        Logger.Info($"({GameInfo.Name}): Finished unpacking base files!");
    }

    public void Launch()
    {
        throw new NotImplementedException();
    }
}
