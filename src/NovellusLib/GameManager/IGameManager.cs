using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovellusLib.GameManager
{
    // need to think about this more
    public interface IGameManager
    {
        string Name { get; }
        bool LaunchSupported { get; }

        void Build();
        void Unpack();
    }
}
