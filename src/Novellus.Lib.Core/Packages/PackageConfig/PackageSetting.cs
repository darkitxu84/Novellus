namespace Novellus.Lib.Core.Packages.PackageConfig;

public record PackageSetting
{
    /// <summary>
    /// ID of the setting, used as key
    /// </summary>
    public required string Id { get; init; }
    
    /// <summary>
    /// Type of setting. Can be: bool, int, float, enum
    /// </summary>
    public required string SettingType { get; init; }
    
    /// <summary>
    /// Display name of the setting
    /// </summary>
    public required string Name { get; init;  }

    /// <summary>
    /// Default value for the setting
    /// </summary>
    public required object Default { get; init; }
    
    /// <summary>
    /// Value for the setting
    /// </summary>
    public required object Value { get; init; }
    
    /// <summary>
    /// (OPTIONAL) Setting description
    /// </summary>
    public string? Description { get; init; }
    
    /// <summary>
    /// (Required / enum) Values of an enum setting.
    /// </summary>
    public EnumEntry[]? Choices { get; init; }

    public Type? GetExpectedType() => SettingType switch
    {
        "bool" or "toggle" => typeof(bool),
        "string" or "text" => typeof(string),
        "enum" or "choice" => typeof(string),
        "int" or "number" => typeof(int),
        "float" or "decimal" => typeof(float),
        _ => null
    };
}