using Novellus.Lib.Core.Packages;

namespace Novellus.Lib.Core.Plugins
{
    public abstract class ModManager(GameInfo gameInfo)
    {
        protected string PathToUnpack { get; } = Path.Combine(Folders.Dumps, gameInfo.Identifier);
        protected GameInfo GameInfo { get; } = gameInfo;
        public abstract Task Build(IEnumerable<IPackage> sortedPackages);
        public abstract Task Unpack();
    }
    
    public interface ILaunchable
    {
        void Launch();
    }
}
