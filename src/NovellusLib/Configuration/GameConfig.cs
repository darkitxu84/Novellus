using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovellusLib.Configuration
{
    public abstract class GameConfig
    {
        public string OutputPath { get; set; } = "";
    }

    public abstract class Ps2GameConfig : GameConfig
    {
        public string ISOPath { get; set; } = "";
        public string PCSX2Path { get; set; } = "";
        public string CheatsPath { get; set; } = "";
        public string TexturesPath { get; set; } = "";
        public string ElfPath { get; set; } = "";
        public bool UseNewPnachFormat { get; set; } = false;
    }

    public abstract class PSPGameConfig : GameConfig
    {
        public string ISOPath { get; set; } = "";
        public string PPSSPPPath { get; set; } = "";
        public string TexturesPath { get; set; } = "";
        public string CheatsPath { get; set; } = "";
    }
}
