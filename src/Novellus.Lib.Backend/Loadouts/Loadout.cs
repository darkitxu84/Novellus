using SharpYaml.Serialization;

namespace Novellus.Lib.Backend.Loadouts;

public class Loadout
{
    [YamlRequired] public string Name { get; set; } = "";
    [YamlRequired] public List<LoadoutEntry> Packages = [];
}