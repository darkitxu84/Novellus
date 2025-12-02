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
            var assembly = Assembly.GetEntryAssembly() 
                ?? throw new Exception("How the fuck the assembly is null?");
            var assemblyLocation = assembly.Location
                ?? throw new Exception("HOW THE FUCK THE ASSEMBLY LOCATION IS NULL!?!?");

            Root = Path.GetDirectoryName(assemblyLocation) 
                ?? throw new Exception("The assembly location is null.");
            Dumps = Path.Combine(Root, "Dumps");
            Packages = Path.Combine(Root, "Packages");
            Libraries = Path.Combine(Root, "Libraries");
            Dependencies = Path.Combine(Root, "Dependencies");
            Config = Path.Combine(Root, "Config");
            Downloads = Path.Combine(Root, "Downloads");
            FilteredCpkCsv = Path.Combine(Dependencies, "FilteredCpkCsv");
        }
    }
}
