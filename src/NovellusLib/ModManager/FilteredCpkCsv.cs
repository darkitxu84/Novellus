using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovellusLib.ModManager
{
    internal static class FilteredCpkCsv
    {
        internal static string[]? Get(string csvName)
        {
            if (!File.Exists(Path.Combine(Folders.FilteredCpkCsv, csvName)))
            {
                Logger.Error($@"Couldn't find CSV file used for unpacking in Dependencies\FilteredCpkCsv: {csvName}");
                return null;
            }

            return File.ReadAllLines(Path.Combine(Folders.FilteredCpkCsv, csvName));
        }
    }
}
