using NovellusLib.Configuration.GameConfigs;
using NovellusLib.FileSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovellusLib.ModManager.Managers
{
    public class Persona3FES : ModManager, ILaunchable
    {
        Persona3FES(ConfigP3F config) => _config = config;

        private readonly ConfigP3F _config;
        public string PathToUnpack => Path.Combine(Folders.Dumps, Game.P3FES.Folder());

        public override Task Build()
        {
            throw new NotImplementedException();
        }

        public override async Task Unpack()
        {
            string pathToUnpack = Path.Combine(Folders.Dumps, Game.P3FES.Folder());
            const string filesFilter = "*.BIN *.PAK *.PAC *.TBL *.SPR *.BF *.BMD *.PM1 *.bf *.bmd *.pm1 *.FPC -r";

            ZipFile.Extract(_config.ISOPath, pathToUnpack, filter: "BTL.CVM DATA.CVM");
            ZipFile.Extract($@"{pathToUnpack}\BTL.CVM", $@"{pathToUnpack}\BTL", filesFilter);
            ZipFile.Extract($@"{pathToUnpack}\DATA.CVM", $@"{pathToUnpack}\DATA", filesFilter);

            File.Delete($@"{pathToUnpack}\BTL.CVM");
            File.Delete($@"{pathToUnpack}\DATA.CVM");

            PAK.ExtractWantedFiles(pathToUnpack);

            return;
        }

        public void Launch()
        {
            throw new NotImplementedException();
        }
    }
}
