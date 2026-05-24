using AtlusFileSystemLibrary;
using AtlusFileSystemLibrary.FileSystems.PAK;
using Novellus.Lib.Backend.FileSystems;
using Novellus.Lib.Backend.Logging;
using Novellus.Lib.Core;
using Novellus.Lib.Core.Packages;

namespace Novellus.Lib.Backend.Mergers;

// TODO: rn the mergers are kind of hardcoded (just spr/spd in this case)
// ideally we should have a more generic system, where we can define "mergable types" and how to merge them
// so we can easily add new types in the future without having to change the merger logic
// we only need spr/spd for now tho, so it's not a big deal, but it's something to keep in mind for the future

public static class PACMerger
{
    // we need to build a map of the files inside the PACS
    // but, we can have a PAC inside a PAC, so we need to build a tree structure to represent this
    // then we build from the leafs to the root, merging the files and the subPACS
    private sealed class PakNode
    {
        public string RelativePath { get; set; } = string.Empty;
        public List<PakNode> Children { get; set; } = [];
        public List<SpriteNode> Sprites { get; set; } = [];
        public List<(string RelativePath, string SourcePath)> Files { get; set; } = [];
    }
    // also, we need to handle spr/spd to merge textures and apply spd patches
    private enum SpriteType { Spr, Spd }
    private sealed class SpriteNode
    {
        public SpriteType Type { get; set; }
        public string? RelativePath { get; set; }
        public List<(string RelativePath, string SourcePath)> Files { get; set; } = [];
    }

    private static bool IsPakFile(string ext) => PAK.FileExtensions.Contains(ext);

    private static bool IsSpriteFile(string ext)
        => ext.Equals(".spr", StringComparison.OrdinalIgnoreCase) || ext.Equals(".spd", StringComparison.OrdinalIgnoreCase);

    private static void ScanIntoNode(string dir, string pakRoot, PakNode node)
    {
        foreach (var entry in Directory.EnumerateFileSystemEntries(dir))
        {

            string relativePath = Path.GetRelativePath(pakRoot, entry);
            string entryExt = Path.GetExtension(entry) ?? ".";

            if (!Directory.Exists(entry))
            {
                node.Files.Add((relativePath, entry));
                continue;
            }

            if (IsPakFile(entryExt))
            {
                var child = new PakNode { RelativePath = relativePath };
                node.Children.Add(child);
                BuildPakNode(entry, child);
            }
            else if (IsSpriteFile(entryExt))
            {
                var child = new SpriteNode
                {
                    RelativePath = relativePath,
                    Type = (entryExt.Equals(".spr", StringComparison.OrdinalIgnoreCase)) ? SpriteType.Spr : SpriteType.Spd
                };
                node.Sprites.Add(child);
                BuildSprNode(entry, child);
            }
            else
                ScanIntoNode(entry, pakRoot, node);
        }
    }

    private static void BuildSprNode(string absoluteDir, SpriteNode node)
    {
        foreach (var file in Directory.EnumerateFiles(absoluteDir, "*", SearchOption.AllDirectories))
        {
            var fileRelative = Path.GetRelativePath(absoluteDir, file);
            node.Files.Add((fileRelative, file));
        }
    }

    private static void BuildPakNode(string absoluteDir, PakNode node)
    {
        foreach (var entry in Directory.EnumerateFileSystemEntries(absoluteDir))
        {
            string relativePath = Path.GetRelativePath(absoluteDir, entry);
            string entryExt = Path.GetExtension(entry) ?? ".";

            if (!Directory.Exists(entry))
            {
                node.Files.Add((relativePath, entry));
                continue;
            }

            if (IsPakFile(entryExt))
            {
                var child = new PakNode { RelativePath = relativePath };
                node.Children.Add(child);
                BuildPakNode(entry, child);
            }
            else if (IsSpriteFile(entryExt))
            {
                var child = new SpriteNode
                {
                    RelativePath = relativePath,
                    Type = (entryExt.Equals(".spr", StringComparison.OrdinalIgnoreCase)) ? SpriteType.Spr : SpriteType.Spd
                };
                node.Sprites.Add(child);
                BuildSprNode(entry, child);
            }
            else ScanIntoNode(entry, absoluteDir, node);

        }
    }

    private static void FindRoots(string dir, string pacFolder, Dictionary<string, PakNode> pacRoots, Dictionary<string, SpriteNode> sprRoots)
    {
        foreach (var entry in Directory.EnumerateDirectories(dir))
        {
            var relativePath = Path.GetRelativePath(pacFolder, entry);
            string ext = Path.GetExtension(entry) ?? ".";

            if (IsPakFile(ext))
            {
                var incoming = new PakNode { RelativePath = relativePath };
                BuildPakNode(entry, incoming);

                if (pacRoots.TryGetValue(relativePath, out var existing))
                    MergeNode(existing, incoming);
                else
                    pacRoots[relativePath] = incoming;
            }
            else if (IsSpriteFile(ext))
            {
                var incoming = new SpriteNode
                {
                    RelativePath = relativePath,
                    Type = (ext.Equals(".spr", StringComparison.OrdinalIgnoreCase)) ? SpriteType.Spr : SpriteType.Spd
                };
                BuildSprNode(entry, incoming);

                if (sprRoots.TryGetValue(relativePath, out var existing))
                    MergeSprNode(existing, incoming);
                else
                    sprRoots[relativePath] = incoming;
            }
            else
                FindRoots(entry, pacFolder, pacRoots, sprRoots);
        }
    }

    private static void MergeSprNode(SpriteNode existing, SpriteNode incoming)
    {
        foreach (var file in incoming.Files)
        {
            var idx = existing.Files.FindIndex(f => f.RelativePath == file.RelativePath);
            if (idx >= 0) existing.Files[idx] = file;
            else existing.Files.Add(file);
        }
    }
    private static void MergeNode(PakNode existing, PakNode incoming)
    {
        foreach (var file in incoming.Files)
        {
            var idx = existing.Files.FindIndex(f => f.RelativePath == file.RelativePath);
            if (idx >= 0) existing.Files[idx] = file;
            else existing.Files.Add(file);
        }

        foreach (var incomingSpr in incoming.Sprites)
        {
            var existingSpr = existing.Sprites.FirstOrDefault(s => s.RelativePath == incomingSpr.RelativePath);
            if (existingSpr == null) existing.Sprites.Add(incomingSpr);
            else MergeSprNode(existingSpr, incomingSpr);
        }

        foreach (var incomingChild in incoming.Children)
        {
            var existingChild = existing.Children.FirstOrDefault(c => c.RelativePath == incomingChild.RelativePath);
            if (existingChild == null) existing.Children.Add(incomingChild);
            else MergeNode(existingChild, incomingChild);
        }
    }
    public static string NormalizePath(IReadOnlySet<string> pacContents, string relativePath)
    {
        relativePath = relativePath.Replace("\\", "/");
        if (pacContents.Contains($"../../../{relativePath}"))
        {
            return $"../../../{relativePath}";
        }
        else if (pacContents.Contains($"../../{relativePath}"))
        {
            return $"../../{relativePath}";
        }
        else if (pacContents.Contains($"../{relativePath}"))
        {
            return $"../{relativePath}";
        }
        return relativePath;
    }
    private static Stream MergeNodeIntoPac(Stream source, PakNode node)
    {
        PAKFileSystem.TryOpen(source, true, out var pac);
        var contents = pac.EnumerateFiles().ToHashSet();

        foreach (var child in node.Children)
        {
            string normalizedPath = NormalizePath(contents, child.RelativePath);
            if (!contents.Contains(normalizedPath))
            {
                Logger.Warn($"PAC '{normalizedPath}' not found in pac '{node.RelativePath}'. Skipping...");
                continue;
            }
            using var childStream = pac.OpenFile(normalizedPath);
            using var mergedChild = MergeNodeIntoPac(childStream, child);
            pac.AddFile(normalizedPath, mergedChild, true, ConflictPolicy.Replace);
        }

        foreach (var (relativePath, sourcePath) in node.Files)
            pac.AddFile(NormalizePath(contents, relativePath), sourcePath, ConflictPolicy.Replace);

        foreach (var spr in node.Sprites)
        {
            string normalizedPath = NormalizePath(contents, spr.RelativePath!);
            if (!contents.Contains(normalizedPath))
            {
                Logger.Warn($"SPR '{normalizedPath}' not found in pac {node.RelativePath}. Skipping...");
                continue;
            }
            var sprStream = pac.OpenFile(normalizedPath);
            var newSpr = SPR.MergeSpr(sprStream, spr.Files, spr.RelativePath!);
            pac.AddFile(normalizedPath, newSpr, true, ConflictPolicy.Replace);
        }

        var savedPacStream = pac.Save();
        pac.Dispose();
        return savedPacStream;
    }

    public static void Merge(IEnumerable<IPackage> packages, string gameId, string outputPath)
    {
        // we should know what PACS are roots (not inside another PAC)
        // so we can load then from disk
        var pakRoots = new Dictionary<string, PakNode>();
        // also handles root spr/spd in PACMerger, is not ideal but meh
        var sprRoots = new Dictionary<string, SpriteNode>();

        foreach (var package in packages)
        {
            string pacFolder = Path.Combine(package.Path, "PAK");
            if (!Directory.Exists(pacFolder)) continue;
            FindRoots(pacFolder, pacFolder, pakRoots, sprRoots);
        }

        foreach (var (relativePath, rootNode) in pakRoots)
        {
            var originalPacPath = Path.Combine(Folders.Dumps, gameId, relativePath);
            var pacBuffer = new MemoryStream();
            using (FileStream fs = new(originalPacPath, FileMode.Open, FileAccess.Read))
                fs.CopyTo(pacBuffer);
            pacBuffer.Position = 0;

            if (!PAKFileSystem.TryOpen(pacBuffer, false, out var pac))
            {
                Logger.Error($"invalid pac bro: {originalPacPath}");
                continue;
            }
            var contents = pac.EnumerateFiles().ToHashSet();

            foreach (var children in rootNode.Children)
            {
                string normalizedPath = NormalizePath(contents, children.RelativePath);
                if (!contents.Contains(normalizedPath))
                {
                    Logger.Warn($"PAC '{normalizedPath}' not found in pac '{originalPacPath}'. Skipping...");
                    continue;
                }
                using var childrenStream = pac.OpenFile(normalizedPath);
                using var savedChildrenStream = MergeNodeIntoPac(childrenStream, children);
                pac.AddFile(children.RelativePath.Replace("\\", "/"), savedChildrenStream, true, ConflictPolicy.Replace);
            }

            foreach (var (RelativePath, SourcePath) in rootNode.Files)
                pac.AddFile(NormalizePath(contents, RelativePath), SourcePath, ConflictPolicy.Replace);

            foreach (var spr in rootNode.Sprites)
            {
                string normalizedPath = NormalizePath(contents, spr.RelativePath!);
                if (!contents.Contains(normalizedPath))
                {
                    Logger.Warn($"SPR '{normalizedPath}' not found in pac {originalPacPath}. Skipping...");
                    continue;
                }
                var sprStream = pac.OpenFile(normalizedPath);
                var newSpr = SPR.MergeSpr(sprStream, spr.Files, spr.RelativePath!);
                pac.AddFile(normalizedPath, newSpr, true, ConflictPolicy.Replace);
            }

            PathUtils.TryCreateDirectory(Path.Combine(outputPath, Path.GetDirectoryName(relativePath)!));
            pac.Save(Path.Combine(outputPath, relativePath));
        }

        foreach (var (relativePath, sprNode) in sprRoots)
        {
            var originalSprPath = Path.Combine(Folders.Dumps, gameId, relativePath);
            var sprOutputPath = Path.Combine(outputPath, relativePath);

            if (!File.Exists(originalSprPath))
            {
                Logger.Warn($"spr {relativePath} not found in dumps, skipping");
                continue;
            }

            Logger.Info($"Merging spr {relativePath}");
            using var ogSprStream = new FileStream(originalSprPath, FileMode.Open, FileAccess.Read);
            using var newSpr = SPR.MergeSpr(ogSprStream, sprNode.Files, sprNode.RelativePath!);

            PathUtils.TryCreateDirectory(Path.GetDirectoryName(sprOutputPath)!);
            using var fileStream = new FileStream(sprOutputPath, FileMode.Create, FileAccess.Write);
            newSpr.CopyTo(fileStream);
        }
    }
}