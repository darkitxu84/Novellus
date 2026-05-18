using Novellus.Lib.Backend.FileSystems;
using Novellus.Lib.Backend.Logging;
using Novellus.Lib.Core.Packages;
using SharpYaml.Serialization;

namespace Novellus.Lib.Backend.Packages;

public class PackageMetadata : IPackageMetadata
{
    // kind of arbitrary values
    private const int NAME_MAX_L = 30;
    private const int ID_MAX_L = 30;
    private const int AUTHOR_MAX_L = 30;
    private const int VERSION_MAX_L = int.MaxValue;
    private const int LINK_MAX_L = int.MaxValue;
    private const int DESCRIPTION_MAX_L = 200;

    [YamlRequired] public string Name { get; init; } = string.Empty;
    [YamlRequired] public string Id { get; init; } = string.Empty;
    [YamlRequired] public string Author { get; init; } = string.Empty;
    [YamlRequired] public string Version { get; set; } = string.Empty;
    [YamlRequired] public string Link { get; init; } = string.Empty;
    [YamlRequired] public string Description { get; init; } = string.Empty;
    public List<string>? Dependencies { get; init; }

    public static PackageMetadata? LoadFromFile(string filePath)
    {
        var pkgMetadata = Yaml.TryLoad<PackageMetadata>(filePath);
        if (pkgMetadata is not null && pkgMetadata.IsValid()) return pkgMetadata;
        return null;
    }

    private bool IsValid()
    {
        static void LogError(string message) => Logger.Error($"Error reading metadata: {message}");

        string[] properties = [Name, Id, Author, Version, Link, Description];
        string[] names = ["Name, Id, Author, Version, Link, Description"];
        int[] maxLength = [NAME_MAX_L, ID_MAX_L, AUTHOR_MAX_L, VERSION_MAX_L, LINK_MAX_L, DESCRIPTION_MAX_L];
        for (var i = 0; i < properties.Length; i++)
        {
            if (string.IsNullOrEmpty(properties[i]))
            {
                LogError($"{names[i]} is null or empty");
                return false;
            }
            if (properties[i].Length > maxLength[i])
            {
                LogError($"{names[i]} is too long, expected less than {maxLength[i]} characters");
                return false;
            }
        }

        if (!Uri.IsWellFormedUriString(Link, UriKind.Absolute))
        {
            LogError($"link is not a valid URL");
            return false;
        }

        // implement version check (idk how aemulus handles versions)

        return true;
    }

    // debug stuff
#if DEBUG
    public override string ToString()
    {
        return
            $"{string.Concat(Enumerable.Repeat("=", 35))} METADATA {string.Concat(Enumerable.Repeat("=", 35))}\n" +
            $"- Name: {Name}\n" +
            $"- Id: {Id}\n" +
            $"- Version: {Version}\n" +
            $"- Author: {Author}\n" +
            $"- Link: {Link}\n" +
            $"- Description: {Description}\n" +
            $"{string.Concat(Enumerable.Repeat("=", 80))}";
    }
#endif
}