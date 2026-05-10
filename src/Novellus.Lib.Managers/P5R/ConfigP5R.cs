using Novellus.Lib.Core.Plugins;
using static Novellus.Lib.Managers.Languages;

namespace Novellus.Lib.Managers.P5R;

public class ConfigP5R : GameConfig
{
    public string CpkName { get; set; } = "mod.cpk";
    public P5RLanguage Language { get; set; } = P5RLanguage.English; 
    public string Version { get; set; } = "1.02"; // consider using a enum later
    public string CpksPath { get; set; } = "";

}
