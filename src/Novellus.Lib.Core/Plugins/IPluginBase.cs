namespace Novellus.Lib.Core.Plugins;

public interface IPluginBase
{
    IEnumerable<IGameIntegration> GetGameIntegrations();
    void OnLoad();
}