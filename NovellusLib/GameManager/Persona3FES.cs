using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovellusLib.GameManager
{
    internal class Persona3FES : IGameManager
    {
        public string Name => "Persona 3 FES";
        public bool LaunchSupported => true;
        public void Build()
        {
            throw new NotImplementedException();
        }
        public void Launch()
        {
            throw new NotImplementedException();
        }
        public void Unpack()
        {
            throw new NotImplementedException();
        }
    }
}
