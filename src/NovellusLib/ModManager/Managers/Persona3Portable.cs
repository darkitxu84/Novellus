using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovellusLib.ModManager.Managers
{
    public class Persona3Portable : ModManager, ILaunchable
    {
        private string PathToUnpack = Path.Combine(Folders.Dumps, Game.P3P.Folder());
        public override Task Build()
        {
            throw new NotImplementedException();
        }
        public override void Launch()
        {
            throw new NotImplementedException();
        }
        public override Task Unpack()
        {
            throw new NotImplementedException();
        }
    }
}
