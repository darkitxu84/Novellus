using Novellus.Lib.Backend.Logging;
using Novellus.Lib.Core.Plugins;

namespace Novellus.Lib.Managers;

public class ManagersIdentifier : IPluginIdentifier
{
    public string Identifier => "novellus.managers";
    public PluginInfo PluginInfo => new PluginInfo("Default Managers", "Tekka & Oceanstuck");
    public IEnumerable<IGameSupport> GetSupportedGames()
    {
        yield return new P1PSP.P1PSPSupport();
        yield return new P3F.P3FSupport();
        yield return new P4.P4Support();
        yield return new P4G32.P4G32Support();
        yield return new P4GVita.P4GVitaSupport();
        yield return new P5.P5Support();
        yield return new P5R.P5RSupport();
        yield return new P5RSwitch.P5RSwitchSupport();
        yield return new PQ.PQSupport();
        yield return new PQ2.PQ2Support();
    }

    public void OnLoad() => Logger.Info("Loading default managers");
    public void OnUnload() => Logger.Info("Unloading default managers");
}