using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Novellus.Lib.Backend;

public static partial class Symlinks
{
    // sadly we don't have CreateHardLink native in C# yet...
    // https://learn.microsoft.com/es-es/dotnet/api/system.io.file.createhardlink?view=net-11.0
    [SupportedOSPlatform("windows")]
    [LibraryImport("kernel32.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool CreateHardLinkA(
        string lpFileName, 
        string lpExistingFileName, 
        IntPtr lpSecurityAttributes
    );
}
