using MoreLinq.Extensions;
using NovellusLib.FileSystems;
using NovellusLib.Logging;
using NovellusLib.Packages;
using NovellusLib.Packages.Definitions;
using System.Text.Json;

namespace NovellusTests;

public record FileEntry(string Relative, string Absolute)
{
    public override string ToString()
    {
        return $"\"{Relative}\": \"{Absolute}\"";
    }
};
public record PAKFile(FileEntry File, List<FileEntry> LooseFiles);

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
        JsonSerializerOptions _jsonSerializerOptions = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip, // consider change the json to json5

        };

        List<PackageConfig> pkgConfigs = [
            new PackageConfig()
            {
                Name = "config1",
                Description = "no description",
                Use = "config1"
            },
            new PackageConfig()
            {
                Name = "config2",
                Description = "no description",
                Use = "config2"
            }
        ];

        string testFile = Path.Combine(Environment.CurrentDirectory, "test.json");
        string json = File.ReadAllText(testFile);
        List<PackageConfig>? configs = JsonSerializer.Deserialize<List<PackageConfig>>(json, _jsonSerializerOptions);

        foreach (var config in configs!)
        {
            Console.WriteLine($"{config.Name} - {config.Use}");
        }

        Logger.Shutdown();
    }
}
