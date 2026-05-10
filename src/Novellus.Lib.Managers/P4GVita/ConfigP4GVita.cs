using Novellus.Lib.Core.Plugins;

namespace Novellus.Lib.Managers.P4GVita;

public sealed class ConfigP4GVita : GameConfig
{
    public string CpkName { get; set; } = "m0.cpk";
    
    [ConfigMetadata("data.cpk Path")] [ConfigFile("CPK File", "*.cpk")]
    public string DataCpkPath { get; set; } = "";
}
