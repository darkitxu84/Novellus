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
            Dumps = $@"{Root}\dumps";
            Packages = $@"{Root}\packages";
            Libraries = $@"{Root}\libraries";
            Dependencies = $@"{Root}\dependencies";
            Config = $@"{Root}\config";
            Downloads = $@"{Root}\downloads";
            FilteredCpkCsv = $@"{Dependencies}\FilteredCpkCsv";
        }
    }
}
