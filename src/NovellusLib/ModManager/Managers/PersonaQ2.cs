using NovellusLib.Configuration.GameConfigs;

namespace NovellusLib.ModManager.Managers;

public class PQ2ModManager(ConfigPQ2 config) : ModManager(Game.PQ2), ILaunchable
{
    public override Task Build()
    {
        throw new NotImplementedException();
    }

    public override Task Unpack()
    {
        throw new NotImplementedException();
    }

    public void Launch()
    {
        throw new NotImplementedException();
    }
}
