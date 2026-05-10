namespace Novellus.Lib.Backend.Loadouts;

public static class LoadoutManager
{
    public static bool IsValidLoadout(Loadout loadout)
    {
        
        return true;
    }
    
    /*
    public static List<Loadout>? GetGameLoadouts(Game game)
    {
        List<Loadout> loadouts = [];
        
        var gameLoadoutsPath = Path.Combine(Folders.Loadouts, game.Folder());
        var loadoutsPaths = Directory.EnumerateDirectories(gameLoadoutsPath)
            .Where(x => x.EndsWith(".yml"));
        
        foreach (string loadoutPath in loadoutsPaths)
        {
            var loadout = Yaml.TryLoad<Loadout>(loadoutPath);
            if (loadout != null)
                loadouts.Add(loadout);
            else
                Logger.Warn($"Could not load loadout '{loadoutPath}'");     
        }
        
        return loadouts;
    }
    */
}