using Novellus.Lib.Backend.FileSystems;
using Novellus.Lib.Backend.Logging;
using Novellus.Lib.Backend.Packages.ConditionInterpreter;
using Novellus.Lib.Core;
using SmartFormat;
using System.Xml.Serialization;

namespace Novellus.Lib.Backend.Packages;

public static class PackageManager
{
    public static bool IsAemPkg(string pkgPath) => File.Exists(Path.Combine(pkgPath, "Package.xml"));
    public static void ConvertAemPkg(string pkgPath)
    {
        Logger.Info("Converting Aemulus package into Novellus format");

        string pkgXml = Path.Combine(pkgPath, "Package.xml");
        XmlSerializer xmlSerializer = new(typeof(Metadata));

        Metadata aemPkg;
        using (FileStream fs = File.Open(pkgXml, FileMode.Open))
        {
            try
            {
                aemPkg = (Metadata)xmlSerializer.Deserialize(fs)!;
            }
            catch (Exception ex)
            {
                Logger.Error($"Cannot convert {pkgPath}: invalid Package.xml ({ex.Message})");
                return;
            }
        }

        PackageMetadata newMeta = new()
        {
            Name = aemPkg.name ?? "I have no name",
            Id = aemPkg.id ?? "ihave.noid",
            Author = aemPkg.author ?? "Someone",
            Version = aemPkg.version ?? "0.69",
            Link = aemPkg.link ?? "www.scaryweb.com",
            Description = aemPkg.description ?? "Novellus can't find a description for this package",
        };

        Package newPkg = new() { Metadata = newMeta, Path = pkgPath };
        Yaml.TrySave(Path.Combine(pkgPath, "package.yml"), newPkg);

        // move package contents to "contents" folder
        string contentsPath = Path.Combine(pkgPath, "contents");
        PathUtils.TryCreateDirectory(contentsPath);

        List<string> directories = [.. Directory.GetDirectories(pkgPath)];
        directories.Remove(contentsPath);

        foreach (var dir in directories)
        {
            string newDir = Path.Combine(contentsPath, Path.GetFileName(dir));
            Logger.Debug($"Trying to move {dir} to {newDir}");
            Directory.Move(dir, newDir);
        }
        File.Delete(pkgXml);

        // rename package folder to package id
        string newPath = Path.Combine(Path.GetDirectoryName(pkgPath)!, newPkg.Metadata.Id);
        Directory.Move(pkgPath, newPath);
    }

    public static Package? LoadFromPath(string pkgPath)
    {
        // TODO: maybe the ui will show some text like:
        // "Novellus detected that you have some Aemulus packages, do you want to convert them?"
        // so maybe move this to another function or create a func "ListAemPkgs"

        if (IsAemPkg(pkgPath))
            ConvertAemPkg(pkgPath);

        Logger.Debug($"Trying to get package.yml from: {pkgPath}");

        var pkgMetadataPath = Path.Combine(pkgPath, "package.yml");
        var pkgMetadata = PackageMetadata.LoadFromFile(pkgMetadataPath);
        if (pkgMetadata is null)
        {
            Logger.Warn($"Cannot load package.yml from {pkgPath}: metadata is invalid");
            return null;
        }

        var pkgConfigPath = Path.Combine(pkgPath, "config.yml");
        var pkgConfig = PackageConfiguration.LoadFromFile(pkgConfigPath);

        var pkg = new Package()
        {
            Metadata = pkgMetadata,
            Configuration = pkgConfig,
            Path = pkgPath
        };
        return pkg;
    }

    public static List<Package> LoadPackagesFromGame(string gameId)
    {
        List<Package> packages = [];

        var gamePkgsPath = Path.Combine(Folders.Packages, gameId);
        foreach (var dir in Directory.EnumerateDirectories(gamePkgsPath, "*", SearchOption.TopDirectoryOnly))
        {
            var pkg = LoadFromPath(dir);
            if (pkg is not null) packages.Add(pkg);
        }
        return packages;
    }

    public static void ProcessPackagesConfiguration(List<Package> packages)
    {
        // we want a dict here cuz some macros use the "id" like MOD_ENABLED()
        Dictionary<string, Package> pkgsById = packages.ToDictionary(p => p.Metadata.Id, p => p);
        Interpreter.SetContext(pkgsById);

        foreach (var pkg in packages)
        {
            if (pkg.Configuration is null) continue;

            var pkgCtx = pkg.Configuration.Settings.ToDictionary(s => s.Id, s => s.Value);

            foreach (var action in pkg.Configuration.Actions)
            {
                if (action.If is null)
                {
                    pkg.ApiCalls.AddRange(action.Do.Select(call => call with
                    {
                        With = call.With
                            .Select(w => Smart.Format(w, pkgCtx))
                            .ToList()
                    }));
                    continue;
                }

                var tokens = Lexer.Tokenize(action.If);
                if (tokens is null)
                {
                    Logger.Warn($"Cannot tokenize condition \"{action.If}\" in package {pkg.Metadata.Id}: invalid syntax");
                    continue;
                }
                var initNode = Parser.Parse(tokens);
                if (initNode is null)
                {
                    Logger.Warn($"Cannot parse condition \"{action.If}\" in package {pkg.Metadata.Id}: invalid syntax");
                    continue;
                }

                bool shouldBeAdded;
                try { shouldBeAdded = Interpreter.ShouldBeAdded(initNode, pkgCtx); }
                catch (InterpreterException ex)
                {
                    Logger.Warn($"Cannot evaluate condition \"{action.If}\" in package {pkg.Metadata.Id}: {ex.Message}");
                    continue;
                }

                if (shouldBeAdded)
                {
                    pkg.ApiCalls.AddRange(action.Do.Select(call => call with
                    {
                        With = call.With
                            .Select(w => Smart.Format(w, pkgCtx))
                            .ToList()
                    }));
                }
            }
        }
    }
}
