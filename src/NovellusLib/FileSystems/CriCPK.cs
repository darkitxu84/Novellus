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
        private class FileToExtract(string fullPath, CpkFile file) : IBatchFileExtractorItem
        {
            public string FullPath { get; set; } = fullPath;
            public CpkFile File { get; set; } = file;
        }

        // maybe we need some benchmarking later, fileList maybe can be optimized with HashSet
        // cuz we're doing Contains checks for every file
        // TODO: implement some percentage calculator, idk how to do that (maybe using a Action parameter?)
        // We can use crifsv2lib.gui for example
        public static void Unpack(string cpkPath, string output, string[]? fileList = null)
        {
            using var fileStream = new FileStream(cpkPath, FileMode.Open);
            using var reader = CriFsLib.Instance.CreateCpkReader(fileStream, true);
            CpkFile[] files = reader.GetFiles();
            fileStream.Close();

            bool extractAll = fileList == null;
            using var extractor = CriFsLib.Instance.CreateBatchExtractor<FileToExtract>(cpkPath, P5RCrypto.DecryptionFunction);
            foreach (var file in files)
            {
                string filePath = string.IsNullOrEmpty(file.Directory) 
                    ? file.FileName 
                    : $"{file.Directory}/{file.FileName}";
                if (!extractAll && !fileList!.Contains(filePath)) continue;
                
                extractor.QueueItem(new FileToExtract(Path.Combine(output, filePath), file));
                Logger.Info($@"Extracting {filePath}");
            }
            extractor.WaitForCompletion();
            ArrayRental.Reset();
        }
    }
}
