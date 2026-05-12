namespace Novellus.Lib.Core.Plugins;

// based on:
// https://api-docs.avaloniaui.net/docs/T_Avalonia_Platform_Storage_FilePickerFileType
// https://api-docs.avaloniaui.net/docs/T_Avalonia_Platform_Storage_FolderPickerOpenOptions

/// <summary>
/// Represents a folder
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class ConfigPathAttribute() : Attribute;

/// <summary>Represents a name mapped to the associated file type (extension)</summary>
/// <param name="name">File type name</param>
/// <param name="glob">List of extensions in GLOB format. I.e. ".png" or ".*"</param>
[AttributeUsage(AttributeTargets.Property)]
public class ConfigFileAttribute(string name, string glob = "") : Attribute
{
    public string Name { get; } = name;
    public string Glob { get; } = glob;
}
