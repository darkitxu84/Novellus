using NovellusLib.Configuration.GameConfigs;
using NovellusLib.FileSystems;

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
        string[]? umd0Files = FilteredCpkCsv.Get(Game.P3P.Folder());
        if (umd0Files is null || umd0Files.Length == 0)
            return;

        await Task.Run(() =>
        {
            Logger.Info($"Extracting umd0.cpk from {config.ISOPath}");
            ZipFile.Extract(config.ISOPath, PathToUnpack, filter: umd0PathFilter);

            Logger.Info($"Extracting files from umd0.cpk");
            CriCPK.Unpack(umd0Path, PathToUnpack, umd0Files);

            Logger.Info("Unpacking extracted files");
            PAK.ExtractWantedFiles(Path.Combine(PathToUnpack, "data"));

            // PathUtils.DeleteIfExists($@"{pathToExtract}\PSP_GAME");
        });
        Logger.Info("[INFO] Finished unpacking base files!");
    }
    public void Launch()
    {
        throw new NotImplementedException();
    }
}
