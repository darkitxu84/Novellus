using NovellusLib.Configuration.GameConfigs;
using NovellusLib.FileSystems;
using NovellusLib.Logging;
using System.IO;
using System.Runtime.CompilerServices;

namespace NovellusLib.ModManager.Managers;

public class P5ModManager(ConfigP5 config) : ModManager(Game.P5), ILaunchable
{
    private readonly string[] _cpkParts =
    {
        Path.Combine(config.EbootPath, "ps3.cpk.66600"),
        Path.Combine(config.EbootPath, "ps3.cpk.66601"),
        Path.Combine(config.EbootPath, "ps3.cpk.66602"),
    };

    public override Task Build()
    {
        throw new NotImplementedException();
    }
    public override async Task Unpack()
    {
        if (!Directory.Exists(config.EbootPath))
        {
            Logger.Error($"Couldn't find {config.EbootPath}. Please correct the file path in config.");
            return;
        }

        string dataCpk = Path.Combine(config.EbootPath, "data.cpk");
        string ps3Cpk = Path.Combine(config.EbootPath, "ps3.cpk");

        await Task.Run(() =>
        {
            var dataFiles = FilteredCpkCsv.Get("filtered_data");
            var ps3Files = FilteredCpkCsv.Get("filtered_ps3");
            if (dataFiles is null || ps3Files is null)
                return;
            
            if (_cpkParts.All(part => File.Exists(part)) && !File.Exists(ps3Cpk)) // merge parts if ps3.cpk doesnt exist
            {
                Logger.Info("Combining ps3.cpk parts");
                using var output = File.OpenWrite(ps3Cpk);
                foreach (var part in _cpkParts)
                {
                    using var input = File.OpenRead(part);
                    input.CopyTo(output);
                }
            }

            TryCreateUnpackDirectory();

            Logger.Info($"Extracting data.cpk");
            CriCPK.Unpack(dataCpk, PathToUnpack, dataFiles);

            Logger.Info($"Extracting data.cpk");
            CriCPK.Unpack(ps3Cpk, PathToUnpack, ps3Files);

            Logger.Info("Unpacking extracted files");
            PAK.ExtractWantedFiles(PathToUnpack);
        });
        Logger.Info($"({Game.P5.Name()}): Finished unpacking base files!");
    }
    public void Launch()
    {
        throw new NotImplementedException();
    }
}
