namespace Novellus.Lib.Core.Packages.PackageConfig;

public record PackageAction
{
    public string? If { get; init; } 
    public required ApiCall[] Do { get; init;  } 
}