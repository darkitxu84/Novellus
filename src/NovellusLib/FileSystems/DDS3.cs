using AtlusFileSystemLibrary.FileSystems.DDS3;
using NovellusLib.Logging;

namespace NovellusLib.FileSystems;

public class DDS3
{
    public void Unpack(string inputFile, string outputPath)
    {
        DDS3FileSystem fs = new();

        try
        {
            fs.Load(inputFile);
        }
        catch (Exception e)
        {
            Logger.Error($"Invalid ddt/img file: {inputFile}.");
            return;
        }
    }
}
