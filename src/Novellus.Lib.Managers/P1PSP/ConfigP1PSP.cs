using Novellus.Lib.Core.Plugins;

namespace Novellus.Lib.Managers.P1PSP;

public sealed class ConfigP1PSP : PSPGameConfig
{
    [ConfigMetadata("Create ISO", "Creates a ISO after build")]
    public bool CreateISO { get; set; } = false;
}
