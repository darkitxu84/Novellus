namespace Novellus.Lib.Backend.Loadouts;

// we don't need to save all metadata in the loadout like aemulus
// just save package id and if it is enabled
public record LoadoutEntry
{
    public string PackageId { get; set; } = "";
    public bool Enabled { get; set; }
}