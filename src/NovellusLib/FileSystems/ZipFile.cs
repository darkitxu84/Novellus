using NovellusLib.Logging;
using System.Diagnostics;

namespace NovellusLib.FileSystems;

public static class ZipFile
{
    // TODO: import dependecies path from config and proper linux support.
    public static void Extract(string zipPath, string outputPath, string filter = "")
    {
        string _7zipPath = Path.Combine(Folders.Dependencies, "7zip", "7z.exe");
        string args = $"x -y -bsp1 \"{zipPath}\" -o\"{outputPath}\" {filter}";

        ProcessStartInfo startInfo = new()
        {
            FileName = _7zipPath,
            Arguments = args,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        using Process process = new() { StartInfo = startInfo };
        process.Start();
        process.WaitForExit();
    }
}
