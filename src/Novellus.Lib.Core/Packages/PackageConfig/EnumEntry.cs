namespace Novellus.Lib.Core.Packages.PackageConfig;

public record EnumEntry
{
    public required string Name { get; init; }
    public required string Value { get; init; }
}