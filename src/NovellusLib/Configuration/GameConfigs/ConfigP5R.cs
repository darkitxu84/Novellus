using static NovellusLib.ModManager.Languages;

namespace NovellusLib.Configuration.GameConfigs;

public class ConfigP5R : GameConfig
{
    public string CpkName { get; set; } = "mod.cpk";
    public P5RLanguage Language { get; set; } = P5RLanguage.English; 
    public string Version { get; set; } = "1.02"; // consider using a enum later
    public string CpksPath { get; set; } = "";

}
