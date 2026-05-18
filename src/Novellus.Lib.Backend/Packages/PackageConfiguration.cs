using Novellus.Lib.Backend.FileSystems;
using Novellus.Lib.Backend.Logging;
using Novellus.Lib.Core.Packages;
using Novellus.Lib.Core.Packages.PackageConfig;
using SharpYaml.Serialization;

namespace Novellus.Lib.Backend.Packages;

public class PackageConfiguration : IPackageConfiguration
{
    // kind of arbitrary 
    private const int ID_MAX_L = 30;
    private const int NAME_MAX_L = 30;
    private const int DESCRIPTION_MAX_L = 40;
    private const int CATEGORY_MAX_L = 20;

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
        static void LogError(string message) => Logger.Error($"Error reading package configuration: {message}");

        foreach (var setting in Settings)
        {
            if (setting.Type is "enum" or "choices" && setting.Choices is null)
            {
                LogError($"Setting '{setting.Id}' has type '{setting.Type}' but no choices defined.");
                return false;
            }

            var expectedType = setting.GetExpectedType();
            if (expectedType is null)
            {
                LogError($"Setting '{setting.Id}' has unknown type '{setting.Type}'.");
                return false;
            }

            try
            {
                setting.Default = Convert.ChangeType(setting.Default, expectedType);
                setting.Value = Convert.ChangeType(setting.Value, expectedType);
            }
            catch (Exception)
            {
                LogError($"Setting '{setting.Id}' has default or current value that cannot be converted to expected type '{expectedType.Name}'.");
                return false;
            }
        }

        return true;
    }

#if DEBUG
    public override string ToString()
    {
        return
            $"{String.Concat(Enumerable.Repeat("=", 35))} CONFIG {String.Concat(Enumerable.Repeat("=", 36))}\n" +
            $"- Settings:\n{string.Join("\n", Settings.Select(s => $"  - {s}"))}\n" +
            $"- Actions:\n{string.Join("\n", Actions.Select(a => $"  - {a}"))}";
    }
#endif
}