using Novellus.Lib.Core.Plugins;
using static Novellus.Lib.Managers.Languages;

namespace Novellus.Lib.Managers.P5RSwitch;

public class ConfigP5RSwitch : GameConfig
{
    public string EmuPath { get; set; } = "";
    public string RomPath { get; set; } = "";
    public P5RLanguage Language { get; set; } = P5RLanguage.English;
    public string CpksPath { get; set; } = "";
}
