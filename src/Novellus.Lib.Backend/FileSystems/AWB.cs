using Novellus.Lib.Backend.Logging;
using Novellus.Lib.Core;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Novellus.Lib.Backend.FileSystems;

// based on alexankitty fork
// https://github.com/alexankitty/AemulusCPMM/blob/main/src/AemulusModManager.Avalonia/Utilities/AWBMerging/AwbMerger.cs

public static class AWB
{
    // TODO: better handling of the external executable dependencies
    public static bool RunAcbEditor(string args)
    {
        string acbEditorPath = Path.Combine(
            Folders.Dependencies,
            "SonicAudioTools",
            OperatingSystem.IsWindows() ? "AcbEditor.exe" : "AcbEditor"
        );
        if (!File.Exists(acbEditorPath))
        {
            Logger.Error($"Couldn't find '{acbEditorPath}'. Please check if it was blocked by your anti-virus.");
            return false;
        }
        ProcessStartInfo startInfo = new()
        {
            FileName = acbEditorPath,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden,
            UseShellExecute = false,
            Arguments = args
        };
        
        using Process process = new();
        process.StartInfo = startInfo;
        process.Start();
        process.WaitForExit();
        return true;
    }

    // TODO: edit awb_unpacker and awb_repacker to take the output path as an argument
    // instead of just dumping it in the executable's directory
    public static void RunAwbUnpacker(string args, string extension)
    {
        string awbUnpackerPath = Path.Combine(
            Folders.Dependencies,
            "AwbTools",
            OperatingSystem.IsWindows() ? "AWB_unpacker.exe" : "AWB_unpacker"
        );
        if (!File.Exists(awbUnpackerPath))
        {
            Logger.Error($"Couldn't find '{awbUnpackerPath}'. Please check if it was blocked by your anti-virus.");
            return;
        }

        ProcessStartInfo startInfo = new()
        {
            FileName = awbUnpackerPath,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden,
            UseShellExecute = false,
            Arguments = args
        };
        using (Process process = new())
        {
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }

        string awbPath = Path.Combine(Path.GetDirectoryName(args)!, Path.GetFileNameWithoutExtension(args));
        Directory.CreateDirectory(awbPath);

        foreach (var file in Directory.EnumerateFiles($"{args}_extracted_files"))
            File.Move(
                file,
                Path.Combine(
                    awbPath,
                    $"{Convert.ToString(int.Parse(Path.GetFileNameWithoutExtension(file), NumberStyles.HexNumber)).PadLeft(5, '0')}_streaming{extension}"
                )
            );
        Directory.Delete($@"{args}_extracted_files", true);
    }

    // TODO: edit awb_unpacker and awb_repacker to take the output path as an argument
    // instead of just dumping it in the executable's directory
    public static void RunAwbRepacker(string args)
    {
        string awbRepackerPath = Path.Combine(
            Folders.Dependencies,
            "AwbTools",
            OperatingSystem.IsWindows() ? "AWB_repacker.exe" : "AWB_repacker"
        );
        if (!File.Exists(awbRepackerPath))
        {
            Logger.Error($"Couldn't find '{awbRepackerPath}'. Please check if it was blocked by your anti-virus.");
            return;
        }

        ProcessStartInfo startInfo = new()
        {
            FileName = awbRepackerPath,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden,
            UseShellExecute = false,
            Arguments = args
        };

        using (Process process = new())
        {
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }


        File.Move(
            Path.Combine(
                Folders.Root,
                "OUT.AWB"
            ),
            Path.ChangeExtension(args, ".awb"),
            true
        );
    }
}
