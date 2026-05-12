using System.ComponentModel;
using Novellus.Lib.Core.Plugins;

namespace Novellus.Lib.Managers.P1PSP;

public sealed class ConfigP1PSP : PSPGameConfig
{
    [DisplayName("Create ISO")]
    [Description("Create a new ISO after build")]
    public bool CreateISO { get; set; } = false;
}
