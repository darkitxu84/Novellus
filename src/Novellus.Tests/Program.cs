using System.Text.Json;
using Novellus.Lib.Backend.FileSystems;
using Novellus.Lib.Backend.Logging;
using Novellus.Lib.Backend.Mergers;
using Novellus.Lib.Backend.Packages;
using Novellus.Lib.Backend.Packages.ConditionInterpreter;
using Novellus.Lib.Backend.Plugins;
using Novellus.Lib.Core;
using Novellus.Lib.Core.Packages.PackageConfig;

namespace NovellusTests;

public class Program
{
    public static void TestLexer()
    {
        const string testCondition =
            "MOD_ENABLED(some.id) and MOD_VERSION(some.id) >= 23.4";

        List<Token>? tokens = Lexer.Tokenize(testCondition);
        var initNode = Parser.Parse(tokens);

        if (tokens != null)
        {
            foreach (Token token in tokens)
            {
                Console.WriteLine(token);
            }
        }
    }
    public static void Main(string[] args)
    {
        //TestLexer();
        Folders.Initialize("C:\\Users\\darki-win\\Documents\\novellus_test");
        Directory.CreateDirectory(Folders.Root);
        Directory.CreateDirectory(Folders.Dumps);
        Directory.CreateDirectory(Folders.Libraries);
        Directory.CreateDirectory(Folders.Dependencies);
        Directory.CreateDirectory(Folders.Config);
        Directory.CreateDirectory(Folders.Downloads);
        Directory.CreateDirectory(Folders.FilteredCpkCsv);
        Directory.CreateDirectory(Folders.Loadouts);
        Directory.CreateDirectory(Folders.Charsets);
        Directory.CreateDirectory(Folders.Plugins);
        Directory.CreateDirectory(Folders.Packages);

        var packages = PackageManager.LoadPackagesFromGame("pq2");
        PackageManager.ProcessPackagesConfiguration(packages);
        AwbMerger.Merge(packages, "pq2", "C:\\Users\\darki-win\\Desktop\\OUTPUT\\NOV");
        Logger.Shutdown();
    }
}