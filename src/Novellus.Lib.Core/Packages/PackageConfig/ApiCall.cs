namespace Novellus.Lib.Core.Packages.PackageConfig;

public record ApiCall
{
    public required string Run { get; init; }
    public required List<string> With { get; init; }
}