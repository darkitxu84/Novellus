using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovellusLib.ModManager.Managers
{
    internal class PersonaQ : ModManager, ILaunchable, IUseFilteredCsv
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
}
