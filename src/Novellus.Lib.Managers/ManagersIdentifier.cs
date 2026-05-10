using Novellus.Lib.Backend.Logging;
using Novellus.Lib.Core.Plugins;

namespace Novellus.Lib.Managers;

public class ManagersIdentifier : IPluginIdentifier
{
    public string Identifier => "novellus.managers";
    public PluginInfo PluginInfo => new PluginInfo("Default Managers", "Tekka & oceanstuck");
    public IEnumerable<IGameIntegration> GetGameIntegrations()
    {
        yield return new P1PSP.P1PSPIntegration();
        yield return new P3F.P3FIntegration();
        yield return new P3P.P3PIntegration();
        yield return new P4.P4Integration();
        yield return new P4G32.P4G32Integration();
        yield return new P4GVita.P4GVitaIntegration();
        yield return new P5.P5Integration();
        yield return new P5R.P5RIntegration();
        yield return new P5RSwitch.P5RSwitchIntegration();
        yield return new PQ.PQIntegration();
        yield return new PQ2.PQ2Integration();
    }

    public void OnLoad() => Logger.Info("Loading default managers");
    public void OnUnload() => Logger.Info("Unloading default managers");
}