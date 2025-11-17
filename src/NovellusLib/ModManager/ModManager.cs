using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovellusLib.ModManager
{
    // need to think about this more
    public abstract class ModManager(Game game)
    {
        protected string PathToUnpack { get; } = Path.Combine(Folders.Dumps, game.Folder());
        protected string PackagesPath { get; } = Path.Combine(Folders.Packages, game.Folder());
        public abstract Task Build();
        public abstract Task Unpack();
    }
    
    public interface ILaunchable
    {
        void Launch();
    }
}
