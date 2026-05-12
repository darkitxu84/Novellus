namespace Novellus.Lib.Core.Plugins;

public interface IPluginInfo
{
    public string Id { get; }
    public string Name { get; }
    public string? Description { get; }
    public string Author { get; }
    public string Dll { get; }
    public string Version { get; }
    public string? PluginPath { get; }
}