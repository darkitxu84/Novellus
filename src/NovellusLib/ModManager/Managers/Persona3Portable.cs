using NovellusLib.Configuration.GameConfigs;
using NovellusLib.FileSystems;
using NovellusLib.Logging;

namespace NovellusLib.ModManager.Managers;

public class P3PModManager(ConfigP3P config) : ModManager(Game.P3P), ILaunchable
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

        const string umd0PathFilter = @"PSP_GAME\USRDIR\umd0.cpk";
        string umd0Path = Path.Combine(PathToUnpack, umd0PathFilter);

        await Task.Run(() =>
        {
            var umd0Files = FilteredCpkCsv.Get("filtered_umd0");
            if (umd0Files is null)
                return;

            TryCreateUnpackDirectory();

            Logger.Info($"Extracting umd0.cpk from {config.ISOPath}");
            ZipFile.Extract(config.ISOPath, PathToUnpack, filter: umd0PathFilter);

            Logger.Info($"Extracting files from umd0.cpk");
            CriCPK.Unpack(umd0Path, PathToUnpack, umd0Files);

            Logger.Info("Unpacking extracted files");
            PAK.ExtractWantedFiles(Path.Combine(PathToUnpack, "data"));

            PathUtils.TryDeleteDirectory(Path.Combine(PathToUnpack, "PSP_GAME"));
        });
        Logger.Info($"({Game.P3P.Name()}): Finished unpacking base files!");
    }
    public void Launch()
    {
        throw new NotImplementedException();
    }
}
