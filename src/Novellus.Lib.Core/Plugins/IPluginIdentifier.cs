namespace Novellus.Lib.Core.Plugins;

public record PluginInfo(string Name, string Author);

public interface IPluginIdentifier
{
    public string Identifier { get; }
    PluginInfo PluginInfo { get; }
    IEnumerable<IGameIntegration> GetGameIntegrations();
    void OnLoad();
    void OnUnload();
}