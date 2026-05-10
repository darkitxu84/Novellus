using Novellus.Lib.Backend.Packages;
using Novellus.Lib.Core.Packages;
using Novellus.Lib.Core.Plugins;

namespace Novellus.Lib.Managers.P4G32;

internal static class P4G32Info
{
    private const string ID = "p4g32";
    private const string NAME = "Persona 4 Golden (32bit)";
    internal static readonly GameInfo GameInfo = new GameInfo(ID, NAME);
}

internal sealed class P4G32Support : IGameSupport
{
    public GameInfo Game => P4G32Info.GameInfo;
    public Type ConfigType => typeof(ConfigP4G32);
    public Type ManagerType => typeof(P4G32Manager);
}

public sealed class P4G32Manager(ConfigP4G32 config): ModManager(P4G32Info.GameInfo), ILaunchable
{
    public override Task Build(IEnumerable<IPackage> sortedPackages)
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
