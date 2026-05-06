using SharpYaml.Serialization;

namespace NovellusLib.Packages;

public class Package : YamlClass
{
    public PackageMetadata Metadata { get; set; } = new PackageMetadata();
    public PackageConfiguration? Configuration { get; set; } = new PackageConfiguration();
    [YamlIgnore] public string PkgPath { get; set; } = "";
    
    // debug stuff
#if DEBUG
    public override string ToString()
    {
        return
            $"{String.Concat(Enumerable.Repeat("=", 35))} PACKAGE {String.Concat(Enumerable.Repeat("=", 36))}\n" +
            $"- PkgPath: {PkgPath}\n" +
            $"- Configs:\n" +
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
        
        Package pkg = new()
        {
            PkgPath = Environment.CurrentDirectory,
            Metadata = pkgMeta,
        };

        // PackageManager.SaveJsonToPath(pkg);
    }
#endif
}
