using Novellus.Lib.Backend.FileSystems;
using Novellus.Lib.Backend.Logging;
using Novellus.Lib.Core.Plugins;
using SharpYaml.Serialization;

namespace Novellus.Lib.Backend.Plugins;

public class PluginInfo : IPluginInfo
{
    [YamlRequired] public required string Id { get; init; }
    [YamlRequired] public required string Name { get; init; }
    [YamlRequired] public required string Author { get; init; }
    [YamlRequired] public required string Dll { get; init; }
    [YamlRequired] public required string Version { get; init; }
    public string? Description { get; init; }
    [YamlIgnore] public string? PluginPath { get; set; }

    private bool IsValid(string filePath)
    {
        // todo: validations!!! yeey!!
        var dir = Path.GetDirectoryName(filePath);
        return File.Exists(Path.Combine(dir!, Dll));
    }

    public static IPluginInfo? LoadFromFile(string filePath)
    {
        var pluginInfo = Yaml.TryLoad<PluginInfo>(filePath);
        if (pluginInfo is not null && pluginInfo.IsValid(filePath))
        {
            pluginInfo.PluginPath = Path.GetDirectoryName(filePath);
            return pluginInfo;
        }
        Logger.Error($"Cannot load plugin information file '{filePath}'. See logs for details.");
        return null;
    }
}