using CriFsV2Lib;
using CriFsV2Lib.Definitions.Interfaces;
using CriFsV2Lib.Definitions.Structs;
using CriFsV2Lib.Definitions.Utilities;
using CriFsV2Lib.Encryption.Game;

namespace NovellusLib.FileSystems
{
    /// <summary> Class for handling CriCPK file system operations.
    /// <para>Used in most of the Atlus games after Persona 3 Portable</para>
    /// <para>For more info, see: <see href="https://amicitia.miraheze.org/wiki/CPK"></see></para>
    /// </summary>
    public static class CriCPK
    {
        private class FileToExtract : IBatchFileExtractorItem
        {
            public string FullPath { get; set; }
            public CpkFile File { get; set; }
            public FileToExtract(string _fullPath, CpkFile _file)
            {
                FullPath = _fullPath;
                File = _file;
            }
        }
        // maybe we need some benchmarking later, fileList maybe can be optimized with HashSet
        // cuz we're doing Contains checks for every file
        public static void Unpack(string cpkPath, string output, string[]? fileList = null)
        {

        }
    }
}
