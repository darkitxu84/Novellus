using System.Diagnostics;

using Novellus.Lib.Backend;
using Novellus.Lib.Backend.Build.BinaryPatching;
using Novellus.Lib.Backend.Logging;
using Novellus.Lib.Backend.Packages;
using Novellus.Lib.Backend.Packages.ConditionInterpreter;
using Novellus.Lib.Core;

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
        Folders.Initialize("C:\\novellus_test");

        var packages = PackageManager.LoadPackagesFromGame("p3fes");
        PackageManager.ProcessPackagesConfiguration(packages);
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        //PACMerger.Merge(packages, "C:\\novellus_test\\Output\\NOV");
        GameFileService.Register(
            relativePath => Path.Combine(Folders.Dumps, "p3fes", relativePath)
         );
        BinaryPatcher.Patch(packages, "C:\\novellus_test\\Output\\NOV");
        //TextureOverride.Process(packages, "C:\\novellus_test\\Output\\TEXTURES_NOV");
        //AwbMerger.Merge(packages, "pq2", "C:\\novellus_test\\Output\\NOV");

        stopwatch.Stop();
        Logger.Info($"Merged in {stopwatch.Elapsed.Seconds}.{stopwatch.ElapsedMilliseconds}s!");
        Logger.Shutdown();
    }
}