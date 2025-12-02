using System.Text.Json;
using System.Text.Json.Serialization;

namespace NovellusLib.Packages;

public class PackageConfig
{
    public string Name { get; set; }
    public string Description { get; set; }
    public bool Enabled { get; set; }
    public string Use {  get; set; }

#if DEBUG
    public override string ToString()
    {
        return
            $"\t- Name: {Name}\n" +
            $"\t\t- Description: {Description}\n" +
            $"\t\t- Enabled: {Enabled}\n" +
            $"\t\t- Use Folder: {Use}\n";
    }
#endif
}
