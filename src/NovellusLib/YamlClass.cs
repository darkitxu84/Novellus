using NovellusLib.Logging;
using SharpYaml.Serialization;

namespace NovellusLib;

public abstract class YamlClass
{
    [YamlExtensionData] 
    private Dictionary<string, object?>? UnkFields { get; set; } = null;

    // for validating jsons
    public void LogUnkFields(string header)
    {
        if (UnkFields is null || UnkFields.Count < 1)
            return;
        
        Logger.Warn(header);
        foreach (var field in UnkFields!)
        {
            Logger.Warn($"   - {field.Key}: {field.Value}");
        }
    }
}
