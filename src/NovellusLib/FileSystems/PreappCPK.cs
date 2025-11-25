using NovellusLib.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovellusLib.FileSystems
{
    public readonly struct AppendInfo(int index, string filePath)
    {
        public int Index { get; } = index;
        public string FilePath { get; } = filePath;
    }
    /// <summary>
    /// Class for handling Preapp Partners CPK Files. Used in Persona 4 Golden 32 bits.
    /// </summary>
    public static class PreappCPK
    {
        public static bool Unpack(string input, string output = ".", string? filter = null)
        {
            if (!File.Exists(input))
            {
                Logger.Error($"Could not unpack file: {input}. The file does not exist.");
                return false;
            }

            return true;
        }

        public static bool Append(string input, AppendInfo appendInfo, string output = ".", string? filter = null)
        {
            if (!File.Exists(input) || !File.Exists(appendInfo.FilePath))
            {
                Logger.Error($"Could not append to file: {input}");
                return false;
            }

            return true;
        }
    }
}
