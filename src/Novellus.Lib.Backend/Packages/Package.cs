using Novellus.Lib.Core.Packages;
using NovellusLib.Packages;
using SharpYaml.Serialization;

namespace Novellus.Lib.Backend.Packages;

public class Package : IPackage
{
    public IPackageMetadata Metadata { get; set; } = new PackageMetadata();
    public IPackageConfiguration? Configuration { get; set; } = new PackageConfiguration();
    [YamlIgnore] public string Path { get; set; } = "";
    
    // debug stuff
#if DEBUG
    public override string ToString()
    {
        return
            $"{String.Concat(Enumerable.Repeat("=", 35))} PACKAGE {String.Concat(Enumerable.Repeat("=", 36))}\n" +
            $"- PkgPath: {Path}\n" +
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
            Path = Environment.CurrentDirectory,
            Metadata = pkgMeta,
        };

        // PackageManager.SaveJsonToPath(pkg);
    }
#endif
}
