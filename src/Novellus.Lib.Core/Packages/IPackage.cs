using Novellus.Lib.Core.Packages.PackageConfig;

namespace Novellus.Lib.Core.Packages;

public interface IPackage
{
    public IPackageMetadata Metadata { get; }
    public IPackageConfiguration? Configuration { get; }
    public string Path { get; } 
    public List<ApiCall> ApiCalls { get; }
}