using Novellus.Lib.Core.Packages;
using Novellus.Lib.Core.Packages.PackageConfig;
using SharpYaml.Serialization;

namespace Novellus.Lib.Backend.Packages;

public sealed class Package : IPackage
{
    public PackageMetadata Metadata { get; set; } = new PackageMetadata();
    public PackageConfiguration? Configuration { get; set; } = new PackageConfiguration();
    [YamlIgnore] public string Path { get; set; } = "";
    [YamlIgnore] public List<ApiCall> ApiCalls { get; set; } = [];

    // the gui should be able to edit the metadata and configuration of the package,
    // but the managers should only be able to read them, so we have explicit interface implementations here
    IPackageMetadata IPackage.Metadata => Metadata;
    IPackageConfiguration? IPackage.Configuration => Configuration;

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
