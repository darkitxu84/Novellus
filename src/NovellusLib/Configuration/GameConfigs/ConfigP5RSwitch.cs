using static NovellusLib.ModManager.Languages;

namespace NovellusLib.Configuration.GameConfigs;

public class ConfigP5RSwitch : GameConfig
{
    public string EmuPath { get; set; } = "";
    public string RomPath { get; set; } = "";
    public P5RLanguage Language { get; set; } = P5RLanguage.English;
    public string CpksPath { get; set; } = "";
}
