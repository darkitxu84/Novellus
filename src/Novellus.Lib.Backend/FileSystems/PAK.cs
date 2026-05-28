using AtlusFileSystemLibrary.Common.IO;


using AtlusFileSystemLibrary.FileSystems.PAK;
using Novellus.Lib.Backend.Logging;
using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;

namespace Novellus.Lib.Backend.FileSystems
{
    // Code adapted from AtlusFileSystemLibrary/PAKPack by tge-was-taken
    // https://github.com/tge-was-taken/AtlusFileSystemLibrary/

    /// <summary>Class for handling PAK file system operations.
    /// <para>Used primarily for Atlus PS2 games and others like Persona 5 and Persona 4 Golden (vita).</para>
    /// <para>For more info, see: <see href="https://amicitia.miraheze.org/wiki/PAC"/></para>
    /// </summary>
    public static class PAK
    {
        // safe for parallel use
        public static readonly FrozenSet<string> FileExtensions =
            new[] { ".bin", ".f00", ".f01", ".p00", ".p01", ".fpc", ".pak", ".pac", ".pack", ".se", ".arc", ".abin", ".se", ".pse", ".tpc" }
                .ToFrozenSet(StringComparer.OrdinalIgnoreCase);

        // wanted extensions when we're extracting the game files
        public static readonly FrozenSet<string> WantedExtensions =
            new[] { ".bf", ".bmd", ".pm1", ".acb", ".awb", ".ctd", ".ftd", ".dat", ".spd", ".gtx" }
                .ToFrozenSet(StringComparer.OrdinalIgnoreCase);

        public static bool IsPak(string ext) => FileExtensions.Contains(ext);
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

        public static bool UnpackFromFile(string inputPath, string? outputPath = null)
        {
            outputPath ??= Path.ChangeExtension(inputPath, null);
            Directory.CreateDirectory(outputPath);

            if (!File.Exists(inputPath) || PAKFileSystem.TryOpen(inputPath, out var pak))
                return false;

            using (pak)
            {
                foreach (string file in pak.EnumerateFiles())
                {
                    var normalizedFilePath = file.Replace("../", "");
                    using var stream = FileUtils.Create(Path.Combine(outputPath, normalizedFilePath));
                    using var inputStream = pak.OpenFile(file);
                    inputStream.CopyTo(stream);
                }
            }

            return true;
        }

        // Probably we don't need this function anymore
        public static void ExtractWantedFiles(string directory)
        {
            if (!Directory.Exists(directory))
                return;

            var files = Directory.EnumerateFiles(directory, "*.*", SearchOption.AllDirectories)
                .Where(f => FileExtensions.Contains(Path.GetExtension(f)));

            Parallel.ForEach(files, file =>
            {
                if (!PAKFileSystem.TryOpen(file, out var pak))
                {
                    Logger.Warn($"Skipping unpacking {file}: not a valid PAK");
                    return;
                }

                List<string> contents = [.. pak.EnumerateFiles()];

                // Check if there are any files we want (or files that could have files we want) and unpack them if so
                bool containersFound = contents.Exists(x => FileExtensions.Contains(Path.GetExtension(x)));

                if (containersFound || contents.Exists(x => WantedExtensions.Contains(Path.GetExtension(x))))
                {
                    Logger.Info($"Unpacking {file}");

                    string outputPath = Path.ChangeExtension(file, null);
                    PathUtils.TryCreateDirectory(outputPath);

                    using (pak)
                    {
                        foreach (var fileInside in contents)
                        {
                            var normalizedFilePath = fileInside.Replace("../", "");
                            var filePath = Path.Combine(outputPath, normalizedFilePath);
                            // There are duplicated PACs because Atlus. Don't try to create a new file if already exists.
                            if (File.Exists(filePath))
                                continue;

                            using var stream = FileUtils.Create(filePath);
                            using var inputStream = pak.OpenFile(fileInside);
                            inputStream.CopyTo(stream);
                        }
                    }

                    // Search the location of the unpacked container for wanted files
                    if (containersFound)
                        ExtractWantedFiles(Path.Combine(Path.GetDirectoryName(file)!, Path.GetFileNameWithoutExtension(file)));
                }
            });
        }
    }
}
