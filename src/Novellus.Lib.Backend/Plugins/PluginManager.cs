using Novellus.Lib.Backend.Logging;
using Novellus.Lib.Core;
using Novellus.Lib.Core.Plugins;
using System.Runtime.Loader;

namespace Novellus.Lib.Backend.Plugins;

public static class PluginManager
{
    private static readonly Dictionary<string, IGameIntegration> GameIntegrations = new(); // <game.id, GameIntregation>
    private static readonly Dictionary<string, IPluginInfo> PluginsInfo = new(); // <plugin.id, PluginInfo>

    public static List<GameInfo> GetSupportedGames()
    {
        List<GameInfo> games = [];
        games.AddRange(GameIntegrations.Values.Select(game => game.Game));
        return games;
    }

    public static IGameIntegration? GetGameIntegration(string gameId)
        => GameIntegrations.GetValueOrDefault(gameId);

    public static IPluginInfo? GetPluginInfo(string pluginId)
        => PluginsInfo.GetValueOrDefault(pluginId);

    public static List<IPluginInfo> GetLoadedPluginsInfo()
        => PluginsInfo.Values.ToList();

    public static void LoadPlugins()
    {
        if (!Directory.Exists(Folders.Plugins)) return;
        var dirs = Directory.GetDirectories(Folders.Plugins);

        foreach (var dir in dirs)
        {
            var ymlPath = Path.Combine(dir, "plugin.yml");
            var pluginInfo = PluginInfo.LoadFromFile(ymlPath);
            if (pluginInfo is null)
            {
                Logger.Error($"Could not load plugin information '{ymlPath}'");
                continue;
            }
            var assemblyPath = Path.Combine(dir, pluginInfo.Dll);
            var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath);
            var pluginIdentifiers = assembly.GetTypes().Where(t =>
                t.GetInterfaces().Contains(typeof(IPluginBase)));

            foreach (var type in pluginIdentifiers)
            {
                // TODO: validations
                if (Activator.CreateInstance(type) is IPluginBase plugin)
                {
                    PluginsInfo.TryAdd(pluginInfo.Id, pluginInfo);
                    foreach (var gameIntegration in plugin.GetGameIntegrations())
                        GameIntegrations.TryAdd(gameIntegration.Game.Identifier, gameIntegration);
                    plugin.OnLoad();
                }
            }
        }
    }
}