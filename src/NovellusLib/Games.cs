using NovellusLib.GameManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovellusLib
{
    public enum Games
    {
        Unknown,
        P1PSP,
        P3FES,
        P4,
        P3P,
        P4GVita,
        P5,
        P4G32Bits,
        P5R,
        P5RSwitch,
        P5S,
        PQ,
        PQ2,
        SMT3
    }

    public static class GameExt
    {
        public static string ToString(this Games game)
        {
            return game switch
            {
                Games.P1PSP => "Persona 1 (PSP)",
                Games.P3FES => "Persona 3 FES",
                Games.P4 => "Persona 4",
                Games.P3P => "Persona 3 Portable",
                Games.P4GVita => "Persona 4 Golden (Vita)",
                Games.P5 => "Persona 5",
                Games.P4G32Bits => "Persona 4 Golden (32-Bits)",
                Games.P5R => "Persona 5 Royal (PS4)",
                Games.P5RSwitch => "Persona 5 Royal (Switch)",
                Games.P5S => "Persona 5 Strikers",
                Games.PQ => "Persona Q",
                Games.PQ2 => "Persona Q2",
                Games.SMT3 => "Shin Megami Tensei III: Nocturne",
                _ => "Unknown"
            };
        }

        public static string Folder(this Games game)
        {
            return game switch
            {
                Games.P1PSP => "p1psp",
                Games.P3FES => "p3fes",
                Games.P4 => "p4",
                Games.P3P => "p3p",
                Games.P4GVita => "p4gvita",
                Games.P5 => "p5",
                Games.P4G32Bits => "p4g32bits",
                Games.P5R => "p5r",
                Games.P5RSwitch => "p5rswitch",
                Games.P5S => "p5s",
                Games.PQ => "pq",
                Games.PQ2 => "pq2",
                Games.SMT3 => "smt3",
                _ => "Unknown"
            };
        }
    }
}
