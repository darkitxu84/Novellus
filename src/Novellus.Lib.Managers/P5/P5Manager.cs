using Novellus.Lib.Backend;
using Novellus.Lib.Backend.FileSystems;
using Novellus.Lib.Backend.Logging;
using Novellus.Lib.Core.Packages;
using Novellus.Lib.Core.Plugins;

namespace Novellus.Lib.Managers.P5;

internal static class P5Info
{
    private const string ID = "p5";
    private const string NAME = "Persona 5";
    internal static readonly GameInfo GameInfo = new GameInfo(ID, NAME);
}

internal sealed class P5Integration : IGameIntegration
{
    public GameInfo Game => P5Info.GameInfo;
    public Type ConfigType => typeof(ConfigP5);
    public Type ManagerType => typeof(P5Manager);
}

public sealed class P5Manager(ConfigP5 config) : ModManager(P5Info.GameInfo), ILaunchable
{
    private readonly string[] _cpkParts =
    {
        Path.Combine(config.EbootPath, "ps3.cpk.66600"),
        Path.Combine(config.EbootPath, "ps3.cpk.66601"),
        Path.Combine(config.EbootPath, "ps3.cpk.66602"),
    };

    public override Task Build(IEnumerable<IPackage> sortedPackages)
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

            PathUtils.TryCreateDirectory(PathToUnpack);

            Logger.Info($"Extracting data.cpk");
            CriCPK.Unpack(dataCpk, PathToUnpack, dataFiles);

            Logger.Info($"Extracting data.cpk");
            CriCPK.Unpack(ps3Cpk, PathToUnpack, ps3Files);

            Logger.Info("Unpacking extracted files");
            PAK.ExtractWantedFiles(PathToUnpack);
        });
        Logger.Info($"({GameInfo.Name}): Finished unpacking base files!");
    }
    public void Launch()
    {
        throw new NotImplementedException();
    }
}
