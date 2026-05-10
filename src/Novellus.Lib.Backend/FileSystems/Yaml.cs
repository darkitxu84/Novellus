using Novellus.Lib.Backend.Logging;
using SharpYaml;

namespace Novellus.Lib.Backend.FileSystems;

public static class Yaml
{
    public static readonly YamlSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public static T? TryLoad<T>(string filePath)
    {
        string fileName = Path.GetFileName(filePath);
        
        if (!File.Exists(filePath))
        {
            Logger.Error($"Cannot open {fileName}: file does not exist");
            return default;
        }

        string yaml = File.ReadAllText(filePath);
        T? yamlClass;
        
        try
        {
            yamlClass = YamlSerializer.Deserialize<T>(yaml, SerializerOptions);
        }
        catch (Exception e) 
        {
            // TODO: Proper logging
            Logger.Error($"Cannot parse {fileName}: not a valid yaml file");
            return default;
        }
        
        return yamlClass;
    }

    public static bool TrySave(string filePath, object obj)
    {
        var fileName = Path.GetFileName(filePath);
        string yaml;
        
        try
        {
            yaml = YamlSerializer.Serialize(obj, SerializerOptions);
        }
        catch (Exception e)
        {
            Logger.Error($"Cannot save yaml to {fileName}: {e}");
            return false;
        }
        
        File.WriteAllText(filePath, yaml);
        return true;
    }
}