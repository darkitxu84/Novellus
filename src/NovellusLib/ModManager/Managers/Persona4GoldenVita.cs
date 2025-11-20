using NovellusLib.Configuration.GameConfigs;
using NovellusLib.FileSystems;

namespace NovellusLib.ModManager.Managers;

public class P4GVitaModManager(ConfigP4GVita config) : ModManager(Game.P4GVita)
{
    public override Task Build()
    {
        throw new NotImplementedException();
    }

    public override async Task Unpack()
    {
        if (!File.Exists(config.DataCpkPath))
        {
            Logger.Error($"Couldn't find {config.DataCpkPath}. Please correct the file path.");
            return;
        }

        await Task.Run(() =>
        {
            var dataFiles = FilteredCpkCsv.Get("filtered_p4gdata");
            if (dataFiles is null)
                return;

            Logger.Info($"Extracting data.cpk");
            CriCPK.Unpack(config.DataCpkPath, PathToUnpack, dataFiles);

            Logger.Info("Unpacking extracted files");
            PAK.ExtractWantedFiles(PathToUnpack);
        });
        Logger.Info($"{Game.P4GVita.Name()}: Finished unpacking base files!");
    }
}
