namespace NovellusLib
{
    public enum Game
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
        public static string Name(this Game game)
        {
            return game switch
            {
                Game.P1PSP => "Persona 1 (PSP)",
                Game.P3FES => "Persona 3 FES",
                Game.P4 => "Persona 4",
                Game.P3P => "Persona 3 Portable",
                Game.P4GVita => "Persona 4 Golden (Vita)",
                Game.P5 => "Persona 5",
                Game.P4G32Bits => "Persona 4 Golden (32-Bits)",
                Game.P5R => "Persona 5 Royal (PS4)",
                Game.P5RSwitch => "Persona 5 Royal (Switch)",
                Game.P5S => "Persona 5 Strikers",
                Game.PQ => "Persona Q",
                Game.PQ2 => "Persona Q2",
                Game.SMT3 => "Shin Megami Tensei III: Nocturne",
                _ => "Unknown"
            };
        }

        // maybe rename this to something like "MiniName" 
        public static string Folder(this Game game)
        {
            return game switch
            {
                Game.P1PSP => "p1psp",
                Game.P3FES => "p3fes",
                Game.P4 => "p4",
                Game.P3P => "p3p",
                Game.P4GVita => "p4gvita",
                Game.P5 => "p5",
                Game.P4G32Bits => "p4g32bits",
                Game.P5R => "p5r",
                Game.P5RSwitch => "p5rswitch",
                Game.P5S => "p5s",
                Game.PQ => "pq",
                Game.PQ2 => "pq2",
                Game.SMT3 => "smt3",
                _ => "unknown"
            };
        }
    }
}
