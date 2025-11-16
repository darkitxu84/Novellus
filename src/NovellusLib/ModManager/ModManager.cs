using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovellusLib.ModManager
{
    // need to think about this more
    public abstract class ModManager
    {
        private string PathToUnpack { get; }
        public abstract Task Build();
        public abstract Task Unpack();
    }
    
    public interface ILaunchable
    {
        void Launch();
    }

    public interface IUseFilteredCsv
    {
        string CsvName { get; }
    }
}
