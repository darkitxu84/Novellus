namespace NovellusLib.ModManager.Managers;

public class PQModManager() : ModManager(Game.PQ), ILaunchable, IUseFilteredCsv
{
    public string CsvName => "persona_q_filtered.csv";
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
