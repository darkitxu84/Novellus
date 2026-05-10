using Novellus.Lib.Backend.FileSystems;
using Novellus.Lib.Backend.Logging;
using Novellus.Lib.Core.Packages;
using Novellus.Lib.Core.Packages.PackageConfig;
using SharpYaml.Serialization;

namespace Novellus.Lib.Backend.Packages;

public class PackageConfiguration : IPackageConfiguration
{
    // "actually!" you can have actions without settings
    // but that is kinda pointless so just use required for both properties
    [YamlRequired] public List<PackageSetting> Settings { get; set; } = [];
    [YamlRequired] public List<PackageAction> Actions { get; set; } = [];
    
    public static PackageConfiguration? LoadFromFile(string filePath)
    {
        // if the package does not have a configuration file just skips
        if (!File.Exists(filePath))
            return null;
        
        var pkgConfig = Yaml.TryLoad<PackageConfiguration>(filePath);
        if (pkgConfig is not null && pkgConfig.IsValid()) return pkgConfig;
        Logger.Error($"Cannot load package config file '{filePath}'. See logs for details.");
        return null;
    }

    private bool IsValid()
    {
        return true;
    }
}