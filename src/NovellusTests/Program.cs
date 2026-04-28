using NovellusLib.Logging;
using System.Text.Json;
using NovellusLib.Configuration.GameConfigs;

namespace NovellusTests;

public record FileEntry(string Relative, string Absolute)
{
    public override string ToString()
    {
        return $"\"{Relative}\": \"{Absolute}\"";
    }
};
public record PakFile(FileEntry File, List<FileEntry> LooseFiles);

public class Program
{
    public static List<FileEntry> GetFileEntrys(string relativeTo, string path)
    {
        List<FileEntry> entries = [];
        foreach (var dir in Directory.GetDirectories(path))
        {
            entries.Add(new FileEntry(Path.GetRelativePath(relativeTo, dir), dir));
        }
        return entries;
    }
    public static void Main(string[] args)
    {
        JsonSerializerOptions jsonSerializerOptions = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip, // consider change the json to json5

        };

        ConfigP3F testCfg = new();
        string cfgJson =  JsonSerializer.Serialize(testCfg, jsonSerializerOptions);
        Console.WriteLine(cfgJson);
        File.WriteAllText("cfg.json", cfgJson);

        Logger.Shutdown();
    }
}