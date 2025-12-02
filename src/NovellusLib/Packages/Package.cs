using System.Text.Json;
using System.Text.Json.Serialization;
using NovellusLib.Logging;

namespace NovellusLib.Packages;

public class Package : JsonClass
{
    public bool Enabled { get; set; } = false;
    public PackageMetadata Metadata { get; set; } = new PackageMetadata();
    public List<PackageConfig>? Configs { get; set; }
    [JsonIgnore] 
    public string PkgPath { get; set; } = "";

    // debug stuff
#if DEBUG
    public override string ToString()
    {
        return
            $"{String.Concat(Enumerable.Repeat("=", 35))} PACKAGE {String.Concat(Enumerable.Repeat("=", 36))}\n" +
            $"- Enabled: {Enabled}\n" +
            $"- PkgPath: {PkgPath}\n" +
            $"- Configs:\n" +
            $"{String.Join("\n ", Configs!)}\n" +
            $"{Metadata}\n";
    }

    public static void CreateTestPackage()
    {
        PackageMetadata pkgMeta = new()
        {
            Name = "Jack Sexo HD",
            Id = "jacksexohdfan.testpackage",
            Author = "Jack Sexo HD Fan",
            Version = "0.6.9",
            Link = "https://jacksexo.hd",
            Description = "The best Jack Sexo HD mod ever created!",
            Dependencies = ["jacksexohdfanclub.jacksexolib"]
        };

        PackageConfig pkgConfig = new()
        {
            Name = "4kmodels",
            Description = "Use Ultra 4K models of Jack Sexo!",
            Enabled = true,
            Use = "4kmodels"
        };

        Package pkg = new()
        {
            Enabled = true,
            PkgPath = Environment.CurrentDirectory,
            Metadata = pkgMeta,
            Configs = [pkgConfig]
        };

        PkgManager.SaveToPath(pkg);
    }
#endif
}
