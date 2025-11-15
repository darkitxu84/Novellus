using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovellusLib.ModManager
{
    // need to think about this more
    public interface IModManager
    {
        Task Build();
        Task Unpack();
    }
    
    public interface ILaunchable
    {
        bool IsLaunchable => true;
        void Launch();
    }
}
