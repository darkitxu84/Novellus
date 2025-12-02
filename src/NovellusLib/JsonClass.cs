
using NovellusLib.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NovellusLib;

public abstract class JsonClass
{
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? UnkFields { get; set; }

    // for validating jsons
    public void LogUnkFields(string header)
    {
        Logger.Warn(header);
        foreach (var field in UnkFields!)
        {
            Logger.Warn($"   - {field.Key}: {field.Value}");
        }
    }
}
