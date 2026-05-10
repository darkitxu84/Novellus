using Novellus.Lib.Core.Plugins;

namespace Novellus.Lib.Managers.P4G32;

public sealed class ConfigP4G32 : GameConfig
{
    public string ExePath { get; set; }
    public string ReloadedPath { get; set; }
    public bool EmptySND { get; set; }
    public bool UseCpk { get; set; }
    public string CpkLang { get; set; }
}
