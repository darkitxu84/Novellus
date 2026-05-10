using Novellus.Lib.Core.Packages.PackageConfig;

namespace Novellus.Lib.Core.Packages;

public interface IPackageConfiguration
{
    public List<PackageSetting> Settings { get; }
    public List<PackageAction> Actions { get; }
}