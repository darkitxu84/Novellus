using NovellusLib.Configuration.GameConfigs;
using NovellusLib.FileSystems;

namespace NovellusLib.ModManager.Managers;

public class PQ2ModManager(ConfigPQ2 config) : ModManager(Game.PQ2), ILaunchable
{
    public override Task Build()
    {
        throw new NotImplementedException();
    }

    public override async Task Unpack() // this is the same as PQ1's unpacking process, consider move to a shared function
    {
        if (!File.Exists(config.DataCpkPath))
        {
            Logger.Error($"Couldn't find {config.DataCpkPath}. Please correct the file path.");
            return;
        }

        await Task.Run(() =>
        {
            var dataFiles = FilteredCpkCsv.Get("filtered_data_pq2.csv");
            if (dataFiles is null)
                return;

            Logger.Info($"Extracting data.cpk");
            CriCPK.Unpack(config.DataCpkPath, PathToUnpack, dataFiles);

            Logger.Info("Unpacking extracted files");
            PAK.ExtractWantedFiles(PathToUnpack);
        });
        Logger.Info($"{Game.PQ2.Name()}; Finished unpacking base files!");
    }

    public void Launch()
    {
        throw new NotImplementedException();
    }
}
