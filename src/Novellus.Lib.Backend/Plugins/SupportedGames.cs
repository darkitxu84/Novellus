using Novellus.Lib.Core.Plugins;

namespace Novellus.Lib.Backend.Plugins;

public static class SupportedGames
{
    private static Dictionary<string, GameInfo> gameInfos = new();
    private static Dictionary<string, Type> managerRegistry = new();

    public static void RegisterGame(GameInfo gameInfo)
    {
        
    }
}