using System.Xml.Serialization;
using Novellus.Lib.Backend.FileSystems;
using Novellus.Lib.Backend.Logging;
using NovellusLib.Packages;

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

        var pkgMetadataPath = Path.Combine(pkgPath, "package.json");
        var pkgMetadata = PackageMetadata.LoadFromFile(pkgMetadataPath);
        if (pkgMetadata is null)
        {
            Logger.Warn($"Cannot load package.json from {pkgPath}: metadata is invalid");
            return null;
        }
        
        var pkgConfigPath = Path.Combine(pkgPath, "config.json");
        var pkgConfig = PackageConfiguration.LoadFromFile(pkgConfigPath);

        var pkg = new Package()
        {
            Metadata = pkgMetadata,
            Configuration = pkgConfig,
            Path = pkgPath
        };
        return pkg;
    }
    
}
