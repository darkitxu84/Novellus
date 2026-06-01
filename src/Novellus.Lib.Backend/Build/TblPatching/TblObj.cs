namespace Novellus.Lib.Backend.Build.TblPatching;

public sealed class NameSection
{
    public int NamesSize;
    public int PointersSize;
    public List<byte[]> Names = [];
    public List<UInt16> Pointers = [];
}

public sealed class Table
{
    public string TableName = string.Empty;
    public List<Section> Sections = [];
    public List<NameSection> NameSections = [];
}

public sealed class Section
{
    public int Size;
    public byte[] Data = [];
}

public sealed class TablePatch
{
    public string Tbl { get; set; } = string.Empty;
    public int? Section { get; set; }
    public int? Offset { get; set; }
    public string Data { get; set; } = string.Empty;
    public int? Index { get; set; }
    public string Name { get; set; } = string.Empty;
}

public sealed class TablePatches
{
    public int Version { get; set; }
    public List<TablePatch> Patches { get; set; } = [];
}