using AtlusFileSystemLibrary.Common.IO;
using AtlusFileSystemLibrary.FileSystems.PAK;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovellusLib.FileSystems
{
    // Code adapted from AtlusFileSystemLibrary/PAKPack by tge-was-taken
    // https://github.com/tge-was-taken/AtlusFileSystemLibrary/

    /// <summary>Class for handling PAK file system operations.
    /// <para>Used primarily for Atlus PS2 games and others like Persona 5 and Persona 4 Golden (vita).</para>
    /// <para>For more info, see: <see href="https://amicitia.miraheze.org/wiki/PAC"/></para>
    /// </summary>
    public static class PAK
    {
        private static bool TryGetValidPak(string path, [NotNullWhen(true)] out PAKFileSystem? pak)
        {
            pak = null;

            if (!File.Exists(path))
            {
                Logger.Error($"{path} does not exist.");
                return false;
            }
            if (!PAKFileSystem.TryOpen(path, out pak))
            {
                Logger.Error($"{path} is an invalid PAK file.");
                return false;
            }

            return true;
        }

        public static List<string>? GetFileContents(string path)
        {
            if (!TryGetValidPak(path, out var pak))
            {
                Logger.Error($"Could not unpack file: {path}");
                return null;
            }
            using (pak)
            {
                var enumeratedFiles = pak.EnumerateFiles().ToList();
                return enumeratedFiles;
            }
        }

        public static void Unpack(string inputPath, string? outputPath = null)
        {
            outputPath ??= Path.ChangeExtension(inputPath, null);
            Directory.CreateDirectory(outputPath);

            if (!TryGetValidPak(inputPath, out var pak))
            {
                Logger.Error($"Could not unpack file: {inputPath}");
                return;
            }
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
        }

    }
}
