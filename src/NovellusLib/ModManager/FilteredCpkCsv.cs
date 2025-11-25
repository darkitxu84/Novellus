using NovellusLib.Logging;

namespace NovellusLib.ModManager;

internal static class FilteredCpkCsv
{
    internal static HashSet<string>? Get(string csvName)
    {
        string csvPath = Path.Combine(Folders.FilteredCpkCsv, $"{csvName}.csv");
        if (!File.Exists(csvPath))
        {
            Logger.Error($@"Couldn't find CSV file used for unpacking in Dependencies\FilteredCpkCsv: {csvName}");
            return null;
        }

        return [.. File.ReadAllLines(csvPath)];
    }
}
