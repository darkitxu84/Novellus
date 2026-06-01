using Novellus.Lib.Core.Packages;

namespace Novellus.Lib.Core.Plugins
{
    public abstract class ModManager(GameInfo gameInfo)
    {
        protected string PathToUnpack { get; } = Path.Combine(Folders.Dumps, gameInfo.Identifier);
        protected GameInfo GameInfo { get; } = gameInfo;
        protected string id { get; } = gameInfo.Identifier;
        protected virtual string GetFileFromGame(string relativePath) => Path.Combine(Folders.Dumps, GameInfo.Identifier, relativePath);
        public abstract Task Build(IEnumerable<IPackage> sortedPackages);
        public abstract Task Unpack();
    }
    
    public interface ILaunchable
    {
        void Launch();
    }
}
