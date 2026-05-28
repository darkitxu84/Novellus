using SharpYaml.Serialization;

namespace Novellus.Lib.Backend.Build.BinaryPatching;

public sealed class BinaryPatch
{
    [YamlRequired] public string File { get; set; } = string.Empty;
    [YamlRequired] public int Offset { get; set; }
    [YamlRequired] public string Data { get; set; } = string.Empty;
}

public sealed class BinaryPatches
{
    [YamlRequired] public int Version { get; set; }
    [YamlRequired] public List<BinaryPatch> Patches { get; set; } = [];
}
