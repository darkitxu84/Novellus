namespace Novellus.Lib.Core.Packages.PackageConfig;

public record PackageSetting
{
    /// <summary>
    /// ID of the setting, used as key
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// Display name of the setting
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// (OPTIONAL) Setting description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// (OPTIONAL) Setting category, used for grouping settings in the UI
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Type of setting. Can be: bool, int, float, enum
    /// </summary>
    public required string Type { get; set; }

    /// <summary>
    /// Default value for the setting
    /// </summary>
    public required object Default { get; set; }
    
    /// <summary>
    /// Value for the setting
    /// </summary>
    public required object Value { get; set; }
    
    
    /// <summary>
    /// (Required / enum) Values of an enum setting.
    /// </summary>
    public EnumEntry[]? Choices { get; set; }

    public Type? GetExpectedType() => Type switch
    {
        "bool" or "toggle" => typeof(bool),
        "string" or "text" => typeof(string),
        "enum" or "choice" => typeof(string),
        "int" or "number" => typeof(int),
        "float" or "decimal" => typeof(float),
        _ => null
    };
}