using System.ComponentModel;
using Novellus.Lib.Core.Plugins;

namespace Novellus.Lib.Managers.P5;

public sealed class ConfigP5 : GameConfig
{
    [DisplayName("Persona 5 EBOOT.BIN Path")]
    [ConfigFile("PS3 Executable", "EBOOT.BIN")]
    public string EbootPath { get; set; } = "";
    
    [DisplayName("RPCS3 Executable Path")]
    [ConfigFile("RPCS3 Executable")]
    public string RPCS3Path { get; set; } = "";
    
    [DisplayName("CPK Name")]
    public string CpkName { get; set; } = "mod";
}
