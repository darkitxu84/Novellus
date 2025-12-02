using System.Text.Json;
using System.Text.Json.Serialization;

namespace NovellusLib.Packages;

public class PackageMetadata : JsonClass
{
    public string Name { get; set; } = "";
    public string Id { get; set; } = "";
    public string Author { get; set; } = "";
    public string Version { get; set; } = "";
    public string Link { get; set; } = "";
    public string Description { get; set; } = "";
    public List<string> Dependencies { get; set; } = [];

    // debug stuff
#if DEBUG
    public override string ToString()
    {
        return 
            $"{String.Concat(Enumerable.Repeat("=", 35))} METADATA {String.Concat(Enumerable.Repeat("=", 35))}\n" +
            $"- Name: {Name}\n" +
            $"- Id: {Id}\n" +
            $"- Author: {Author}\n" +
            $"- Link: {Link}\n" +
            $"- Description: {Description}\n" +
            $"- Dependencies:\n\t- {String.Join("\n\t- ", Dependencies)}\n" +
            $"{String.Concat(Enumerable.Repeat("=", 80))}";
    }
#endif
}