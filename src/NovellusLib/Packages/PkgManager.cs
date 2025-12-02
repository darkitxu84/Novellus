using NovellusLib.Logging;
using System.Text.Json;
using System.Xml.Serialization;

namespace NovellusLib.Packages;

public static class PkgManager
{
    private readonly static JsonSerializerOptions _jsonSerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip, // consider change the json to json5

    };

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
            Name = aemPkg.name ?? "",
            Id = aemPkg.id ?? "",
            Author = aemPkg.author ?? "",
            Version = aemPkg.version ?? "",
            Link = aemPkg.link ?? "",
            Description = aemPkg.description ?? "",
        };

        Package newPkg = new() { Metadata = newMeta, PkgPath = pkgPath };
        SaveToPath(newPkg);

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
    }

    public static Package? GetFromPath(string pkgPath)
    {
        // TODO: maybe the ui will show some text like:
        // "Novellus detected that you have some Aemulus packages, do you want to convert them?"
        // so maybe move this to another function or create a func "ListAemPkgs"
        if (IsAemPkg(pkgPath))
            ConvertAemPkg(pkgPath);

        Logger.Debug($"Trying to get package.json from: {pkgPath}");

        string pkgFile = Path.Combine(pkgPath, "package.json");
        if (!File.Exists(pkgFile))
        {
            Logger.Error($"Cannot open package.json in {pkgPath}: file does not exist");
            return null;
        }

        string json = File.ReadAllText(pkgFile);
        Package? pkg = JsonSerializer.Deserialize<Package>(json, _jsonSerializerOptions);
        if (pkg is null)
        {
            Logger.Error($"Cannot parse package.json in {pkgPath}: not a valid json");
            return null;
        }
        pkg.PkgPath = pkgPath;

        Validate(pkg);
        return pkg;
    }


    public static void SaveToPath(Package pkg)
    {
        string pkgFile = Path.Combine(pkg.PkgPath, "package.json");
        string json = JsonSerializer.Serialize(pkg, _jsonSerializerOptions);
        File.WriteAllText(pkgFile, json);
    }

    public static void Validate(Package pkg)
    {
        bool isValid = true;

        if (pkg.UnkFields?.Count > 0)
        {
            pkg.LogUnkFields($"Unknow keys in package {pkg.PkgPath}");
            isValid = false;
        }
        if (pkg.Metadata.UnkFields?.Count > 0)
        {
            pkg.LogUnkFields($"Unknow keys in package metada {pkg.PkgPath}");
            isValid = false;
        }

        if (!isValid)
            Logger.Warn($"WARNING!!!! Some values of package {pkg.PkgPath} may been reseted to default!!!");
    }

    public static Dictionary<string, string> GetPkgFiles(Package pkg)
    {
        Dictionary<string, string> filesEntries = [];

        string contentsPath = Path.Combine(pkg.PkgPath, "contents");
        var contentFiles = Directory.EnumerateFiles(contentsPath, "*.*", SearchOption.AllDirectories);
        foreach (var file in contentFiles)
        {
            filesEntries[Path.GetRelativePath(contentsPath, file)] = file;
        }

        if (pkg.Configs is not null)
        {
            foreach (var config in pkg.Configs)
            {
                if (!config.Enabled) continue;

                string usePath = Path.Combine(pkg.PkgPath, config.Use);
                if (!Directory.Exists(usePath))
                {
                    Logger.Warn($"Can't load config {config.Name} in pkg {pkg.Metadata.Name}: {usePath} doesn't exist");
                    continue;
                }

                Logger.Info($"Using config {config.Name} in pkg {pkg.Metadata.Name}");

                var configFiles = Directory.EnumerateFiles(usePath, "*.*", SearchOption.AllDirectories);
                foreach (var file in configFiles)
                {
                    filesEntries[Path.GetRelativePath(usePath, file)] = file;
                }
            }
        }

        return filesEntries;

    }

}
