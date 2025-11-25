using NovellusLib.Configuration.GameConfigs;
using NovellusLib.FileSystems;
using NovellusLib.Logging;
using System.IO;

namespace NovellusLib.ModManager.Managers;

public class P5RModManager(ConfigP5R config) : ModManager(Game.P5R)
{
    private bool CheckNeededCpks()
    {
        List<string> neededCpks =
        [
            "dataR.cpk",
            "ps4R.cpk"
        ];

        if (config.Language != Languages.P5RLanguage.English)
            neededCpks.Add($"dataR_{config.Language.CpkSuffix()}.cpk");

        if (config.Version == ">= 1.02")
        {
            neededCpks.Add("patch2R.cpk");
            if (config.Language != Languages.P5RLanguage.English)
                neededCpks.Add($"patch2R_{config.Language.CpkSuffix()}.cpk");
        }

        var folderCpks = Directory
            .GetFiles(config.CpksPath, "*.cpk", SearchOption.TopDirectoryOnly)
            .Select(file => Path.GetFileName(file))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var missingCpks = neededCpks
            .Where(cpk => !folderCpks.Contains(cpk))
            .ToList();

        if (missingCpks.Count > 0)
        {
            Logger.Error($"Not all cpks needed are found in top directory of {config.CpksPath}");
            Logger.Error($"Missing cpks: {string.Join(", ", missingCpks)}");
            return false;
        }

        return true;
    }

    private void UnpackLocalized()
    {
        if (config.Language != Languages.P5RLanguage.English)
            return;

        var dataRLocalizedFiles = FilteredCpkCsv.Get("filtered_dataR_Localized");
        if (dataRLocalizedFiles is null)
            return;
        var localizedCpk = $"dataR_{config.Language.CpkSuffix()}.cpk";

        Logger.Info("Extracting localized dataR cpk");
        CriCPK.Unpack(localizedCpk, PathToUnpack, dataRLocalizedFiles);
    }

    private void UnpackPatch()
    {
        if (config.Version != ">= 1.02")
            return;

        var patch2RFiles = FilteredCpkCsv.Get("filtered_patch2R");
        if (patch2RFiles is null)
            return;
        var patchCpk = Path.Combine(config.CpksPath, "patch2R.cpk");

        Logger.Info("Extracting patch2R.cpk");
        CriCPK.Unpack(patchCpk, PathToUnpack, patch2RFiles);

        if (config.Language != Languages.P5RLanguage.English)
        {
            var patch2RLocalizedFiles = FilteredCpkCsv.Get($"filtered_patch2R{config.Language.CpkSuffix()}");
            if (patch2RLocalizedFiles is null)
                return;
            var patchLocalizedCpk = Path.Combine(config.CpksPath, $"patch2R{config.Language.CpkSuffix()}.cpk");

            Logger.Info("Extracting localized patch2R.cpk");
            CriCPK.Unpack(patchLocalizedCpk, PathToUnpack, patch2RLocalizedFiles);
        }
    }
    public override Task Build()
    {
        throw new NotImplementedException();
    }

    public override async Task Unpack()
    {
        if (!Directory.Exists(config.CpksPath))
        {
            Logger.Error($"Couldn't find {config.CpksPath}. Please correct the file path.");
            return;
        }
        if (!CheckNeededCpks())
            return;

        await Task.Run(() =>
        {
            var dataRFiles = FilteredCpkCsv.Get("filtered_dataR");
            var ps4RFiles = FilteredCpkCsv.Get("filtered_ps4R");
            if (dataRFiles is null || ps4RFiles is null)
                return;

            TryCreateUnpackDirectory();

            Logger.Info($"Extracting dataR.cpk");
            CriCPK.Unpack(Path.Combine(config.CpksPath, "dataR.cpk"), PathToUnpack, dataRFiles);

            Logger.Info($"Extracting ps4R.cpk");
            CriCPK.Unpack(Path.Combine(config.CpksPath, "ps4R.cpk"), PathToUnpack, ps4RFiles);

            UnpackLocalized();
            UnpackPatch();

            Logger.Info("Unpacking extracted files");
            PAK.ExtractWantedFiles(PathToUnpack);

        });
        Logger.Info($"({Game.P5R.Name()}): Finished unpacking base files!");

    }
}
