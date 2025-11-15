using NovellusLib.Configuration.GameConfigs;
using NovellusLib.FileSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovellusLib.ModManager.Managers
{
    public class Persona3FES : IModManager, ILaunchable
    {
        Persona3FES(ConfigP3F config) => _config = config;

        private ConfigP3F _config;
        public Task Build()
        {
            throw new NotImplementedException();
        }
        public void Launch()
        {
            throw new NotImplementedException();
        }
        public Task Unpack()
        {
            string pathToUnpack = Path.Combine(Folders.Dumps, GameExt.Folder(Game.P3FES));
            const string filesFilter = "*.BIN *.PAK *.PAC *.TBL *.SPR *.BF *.BMD *.PM1 *.bf *.bmd *.pm1 *.FPC -r";

            ZipFile.Extract(_config.ISOPath, pathToUnpack, filter: "BTL.CVM DATA.CVM");
            ZipFile.Extract($@"{pathToUnpack}\BTL.CVM", $@"{pathToUnpack}\BTL", filesFilter);
            ZipFile.Extract($@"{pathToUnpack}\DATA.CVM", $@"{pathToUnpack}\DATA", filesFilter);

            File.Delete($@"{pathToUnpack}\BTL.CVM");
            File.Delete($@"{pathToUnpack}\DATA.CVM");

            return Task.CompletedTask;
        }
    }
}
