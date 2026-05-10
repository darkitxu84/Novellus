namespace Novellus.Lib.Core.Packages;

public interface IPackageMetadata
{
    public string Name { get; }
    public string Id { get; }
    public string Author { get; }
    public string Version { get; }
    public string Link { get; }
    public string Description { get; }
    public List<string>? Dependencies { get; }
}