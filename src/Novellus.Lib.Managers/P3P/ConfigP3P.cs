using System.ComponentModel;
using Novellus.Lib.Core.Plugins;

namespace Novellus.Lib.Managers.P3P;

public sealed class ConfigP3P : PSPGameConfig
{
    [DisplayName("CPK Name")]
    public string CpkName { get; set; } = "mod.cpk";
}
