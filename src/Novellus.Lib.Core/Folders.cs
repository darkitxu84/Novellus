namespace Novellus.Lib.Core;

public static class Folders
{
    private static bool _initialized;
    
    // use null! to supress warning, the backend/gui must call Initialize function
    public static string Root { get; private set; } = null!;
    public static string Dumps { get; private set; } = null!;
    public static string Packages { get; private set; } = null!;
    public static string Libraries { get; private set; } = null!;
    public static string Dependencies  { get; private set; } = null!;
    public static string Config {get; private set;} = null!;
    public static string Downloads { get; private set; } = null!;
    public static string FilteredCpkCsv { get; private set; } = null!;
    public static string Loadouts { get; private set; } = null!;
    public static string Charsets { get; private set; } = null!;
    public static string Plugins  { get; private set; } = null!;

    public static void Initialize(string root)
    {
        if (_initialized)
            throw new InvalidOperationException("The folders can only be initialized once.");
        
        _initialized = true;

        Root = root;
        Dumps = Path.Combine(Root, "Dumps");
        Packages = Path.Combine(Root, "Packages");
        Libraries = Path.Combine(Root, "Libraries");
        Dependencies = Path.Combine(Root, "Dependencies");
        Config = Path.Combine(Root, "Config");
        Downloads = Path.Combine(Root, "Downloads");
        FilteredCpkCsv = Path.Combine(Dependencies, "FilteredCpkCsv");
        Loadouts = Path.Combine(Dependencies, "Loadouts");
        Charsets = Path.Combine(Dependencies, "Charsets");
        Plugins = Path.Combine(Root, "Plugins");
    }
}