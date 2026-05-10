using System.Runtime.Loader;
using AtlusScriptLibrary.Common.Collections;
using Novellus.Lib.Backend.Logging;
using Novellus.Lib.Core;
using Novellus.Lib.Core.Plugins;

namespace Novellus.Lib.Backend.Plugins;

public static class PluginManager
{
    private static Dictionary<string, IGameSupport> GameSupports = new();
    private static Dictionary<string, PluginInfo> PluginsInfo = new();

    public static List<GameInfo> GetSupportedGames()
    {
        List<GameInfo> games = [];

        foreach (var game in GameSupports.Values) 
            games.Add(game.Game);

        return games;
    }
    
    public static void LoadPlugins()
    {
        if (!Directory.Exists(Folders.Plugins)) return;
        var dlls = Directory.GetFiles(Folders.Plugins, "*.dll");

        foreach (var dll in dlls)
        {
            try
            {
                var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(dll);

                var pluginIdentifiers = assembly.GetTypes().Where(t =>
                    t.GetInterfaces().Contains(typeof(IPluginIdentifier)));

                foreach (var type in pluginIdentifiers)
                {
                    // TODO: validations
                    if (Activator.CreateInstance(type) is IPluginIdentifier pluginIdentifier)
                    {
                        PluginsInfo.TryAdd(pluginIdentifier.Identifier, pluginIdentifier.PluginInfo);
                        foreach (var support in pluginIdentifier.GetSupportedGames())
                            GameSupports.TryAdd(support.Game.Identifier, support);
                        pluginIdentifier.OnLoad();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message); 
            }
        }
    }
}