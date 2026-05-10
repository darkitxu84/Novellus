using Novellus.Lib.Backend;
using Novellus.Lib.Backend.FileSystems;
using Novellus.Lib.Backend.Logging;
using Novellus.Lib.Core.Packages;
using Novellus.Lib.Core.Plugins;

namespace Novellus.Lib.Managers.P3P;

internal static class P3PInfo
{
    private const string ID = "p3p";
    private const string NAME = "Persona 3 Portable (PSP)";
    internal static readonly GameInfo GameInfo = new GameInfo(ID, NAME);
}

internal sealed class P3PSupport : IGameSupport
{
    public GameInfo Game => P3PInfo.GameInfo;
    public Type ConfigType => typeof(ConfigP3P);
    public Type ManagerType => typeof(P3PManager);
}

public sealed class P3PManager(ConfigP3P config) : ModManager(P3PInfo.GameInfo), ILaunchable
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

        const string umd0PathFilter = @"PSP_GAME\USRDIR\umd0.cpk";
        string umd0Path = Path.Combine(PathToUnpack, umd0PathFilter);

        await Task.Run(() =>
        {
            var umd0Files = FilteredCpkCsv.Get("filtered_umd0");
            if (umd0Files is null)
                return;

            PathUtils.TryCreateDirectory(PathToUnpack);

            Logger.Info($"Extracting umd0.cpk from {config.ISOPath}");
            ZipFile.Extract(config.ISOPath, PathToUnpack, filter: umd0PathFilter);

            Logger.Info($"Extracting files from umd0.cpk");
            CriCPK.Unpack(umd0Path, PathToUnpack, umd0Files);

            Logger.Info("Unpacking extracted files");
            PAK.ExtractWantedFiles(Path.Combine(PathToUnpack, "data"));

            PathUtils.TryDeleteDirectory(Path.Combine(PathToUnpack, "PSP_GAME"));
        });
        Logger.Info($"({GameInfo.Name}): Finished unpacking base files!");
    }
    public void Launch()
    {
        throw new NotImplementedException();
    }
}
