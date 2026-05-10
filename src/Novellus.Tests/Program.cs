using System.Text.Json;
using Novellus.Lib.Backend.FileSystems;
using Novellus.Lib.Backend.Logging;
using Novellus.Lib.Backend.Packages;
using Novellus.Lib.Backend.Packages.PackageConfig.ConditionInterpreter;
using Novellus.Lib.Backend.Plugins;
using Novellus.Lib.Core;

namespace NovellusTests;

public class Program
{
    public static void TestLexer()
    {
        const string testCondition =
            "MOD_ENABLED(some.id) and MOD_VERSION(some.id) >= 23.4";
        Lexer lexer = new(testCondition);
        List<Token>? tokens = lexer.Tokenize();
        
        Parser parser = new(tokens);
        parser.Parse();

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
        TestLexer();
        Folders.Initialize(".");
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
        
        PluginManager.LoadPlugins();
        foreach (var game in PluginManager.GetSupportedGames())
        {
            Console.WriteLine($"{game.Identifier}: {game.Name}");
        }
    }
}