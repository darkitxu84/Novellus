using System.Reflection;

namespace NovellusLib
{
    public static class Folders
    {
        public static readonly string Root;
        public static readonly string Dumps;
        public static readonly string Packages;
        public static readonly string Libraries;
        public static readonly string Dependencies;
        public static readonly string Config;
        public static readonly string Downloads;
        public static readonly string FilteredCpkCsv;

        static Folders()
        {
            Root = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            Dumps = Path.Combine(Root, "dumps");
            Packages = Path.Combine(Root, "packages");
            Libraries = Path.Combine(Root, "libraries");
            Dependencies = Path.Combine(Root, "dependencies");
            Config = Path.Combine(Root, "config");
            Downloads = Path.Combine(Root, "downloads");
            FilteredCpkCsv = Path.Combine(Dependencies, "FilteredCpkCsv");
        }
    }
}
