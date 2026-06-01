using Novellus.Lib.Backend.FileSystems;
using Novellus.Lib.Backend.Logging;
using Novellus.Lib.Core.Packages;
using System.Collections.Frozen;
using System.Text;

namespace Novellus.Lib.Backend.Build.TblPatching;

public enum TableKind { Default, PQ }
public enum Endianess { Big, Little }

public static class TblPatcher
{
    private static readonly FrozenSet<string> P4GTables = new[]
    { "SKILL", "UNIT", "MSG", "PERSONA", "ENCOUNT", "EFFECT", "MODEL", "AICALC", "ITEMTBL" }
    .ToFrozenSet();

    private static readonly FrozenSet<string> P3PTables = new[]
        { "SKILL", "UNIT", "MSG", "PERSONA", "ENCOUNT", "EFFECT", "MODEL", "AICALC" }
        .ToFrozenSet();

    private static readonly FrozenSet<string> P3FTables = new[]
        { "SKILL", "SKILL_F", "UNIT", "UNIT_F", "MSG", "PERSONA", "PERSONA_F",
      "ENCOUNT", "ENCOUNT_F", "EFFECT", "MODEL", "AICALC", "AICALC_F" }
        .ToFrozenSet();

    private static readonly FrozenSet<string> P5Tables = new[]
        { "AICALC", "ELSAI", "ENCOUNT", "EXIST", "ITEM", "NAME",
      "PERSONA", "PLAYER", "SKILL", "TALKINFO", "UNIT", "VISUAL" }
        .ToFrozenSet();

    private static readonly FrozenSet<string> PQTables = new[]
        { "battle/table/personanametable.tbl", "battle/table/enemynametable.tbl",
      "battle/table/skillnametable.tbl" }
        .ToFrozenSet();

    private static FrozenSet<string> GetGameTables(string id) => id switch
    {
        "p4g32" or "p4gvita" => P4GTables,
        "p3p" => P3PTables,
        "p3fes" or "p4" => P3FTables,
        "p5" or "p5r" or "p5rswitch" => P5Tables,
        "pq" or "pq2 "=> PQTables,
        _ => throw new ArgumentException($"Unknown game id: {id}")
    };
    private static string ResolveTblPath(string tbl, string gameId, string? cpkLang = null)
    {
        // for pq2 and pq, tbl is just the relative path to the table
        if (gameId is "pq" or "pq2") return tbl;
        if (gameId is "p4g32" or "p4gvita" or "p3p" && tbl.Equals("ITEMTBL")) return "init/itemtbl.bin";

        return $"{tbl}.TBL";
    }
    private static TableKind GetTblKind(string id) => id switch
    {
        "pq" or "pq2" => TableKind.PQ,
        _ => TableKind.Default
    };
    private static Endianess GetEndianess(string id) => id switch
    { 
        "p5" or "p5r" or "p5rswitch" => Endianess.Big,
        _ => Endianess.Little
    };
    private static List<NameSection> GetNameSections(Stream tbl)
    {
        List<NameSection> sections = [];
        byte[] tblBytes = [];
        // we have memory stream when the tbl is inside a PAC, so just use that buffer
        if (tbl is MemoryStream tblMs)
            tblBytes = tblMs.GetBuffer();
        else
        {
            tbl.ReadExactly(tblBytes, 0, (int)tbl.Length);
        }

        int pos = 0;
        NameSection section;
        while (pos < tblBytes.Length)
        {
            section = new NameSection();

            // Get big endian section size
            section.PointersSize = BitConverter.ToInt32(tblBytes[pos..(pos + 4)].Reverse().ToArray(), 0);

            // Get pointers
            byte[] segment = tblBytes[(pos + 4)..(pos + 4 + section.PointersSize)];
            section.Pointers = new List<UInt16>();
            for (int j = 0; j < segment.Length; j += 2)
            {
                section.Pointers.Add(BitConverter.ToUInt16(segment[j..(j + 2)].Reverse().ToArray(), 0));
            }

            // Get to name section
            pos += section.PointersSize + 4;
            if ((pos % 16) != 0)
                pos += 16 - (pos % 16);

            // Get big endian section size
            section.NamesSize = BitConverter.ToInt32(tblBytes[pos..(pos + 4)].Reverse().ToArray(), 0);

            // Get names
            segment = tblBytes[(pos + 4)..(pos + 4 + section.NamesSize)];
            section.Names = new List<byte[]>();
            List<byte> name = new List<byte>();
            foreach (var segmentByte in segment)
            {
                if (segmentByte == (byte)0)
                {
                    section.Names.Add(name.ToArray());
                    name = new List<byte>();
                }
                else
                {
                    name.Add(segmentByte);
                }
            }

            // Get to next section
            pos += section.NamesSize + 4;
            if ((pos % 16) != 0)
                pos += 16 - (pos % 16);

            sections.Add(section);
        }
        return sections;
    }
    private static List<NameSection> GetNameSectionQ(string tbl)
    {
        List<NameSection> sections = new List<NameSection>();
        byte[] tblBytes = File.ReadAllBytes(tbl);
        int pos = 0;
        NameSection section = new NameSection();

        section.PointersSize = BitConverter.ToInt16(SliceArray(tblBytes, pos, pos + 2), 0); //actually number of pointers in q2 nametbls
        byte[] segment = SliceArray(tblBytes, pos + 2, pos + 2 + section.PointersSize * 2);
        section.Pointers = new List<ushort>();
        for (int i = 0; i < segment.Length; i += 2)
        {
            section.Pointers.Add(BitConverter.ToUInt16(SliceArray(segment, i, i + 2), 0));
        }

        pos += section.PointersSize * 2 + 2;
        section.NamesSize = tblBytes.Length - pos;
        segment = SliceArray(tblBytes, pos, pos + section.NamesSize);
        section.Names = new List<byte[]>();
        List<byte> name = new List<byte>();
        foreach (var segmentByte in segment)
        {
            if (segmentByte == (byte)0)
            {
                section.Names.Add(name.ToArray());
                name = new List<byte>();
            }
            else
            {
                name.Add(segmentByte);
            }
        }
        section.Names.Add(Encoding.ASCII.GetBytes("owo what\'s this")); // last ptr goes past eof, add dummy name for convenience later

        sections.Add(section);
        return sections;
    }

    internal static string GetTblsPath(string gameId) => gameId switch
    {
        "p3fes" or "p4" => "BTL/BATTLE",
        "p5rswitch" => "BASE/BATTLE/TABLE",
        "p3p" => "data/init_free.bin",
        "p5" or "p5r" => "battle/table.pac",
        "p4g32" or "p4gvita" => "init_free.bin/battle",
        "pq" or "pq2" => "",
        _ => throw new ArgumentException($"Unknow game id: {gameId}")
    };

    public static void Patch(IEnumerable<IPackage> packages, string gameId, string outputPath, string? cpkLang = null)
    {
        var tblsPath = GetTblsPath(gameId);
        var tables = GetGameTables(gameId);
        var endianess = GetEndianess(gameId);
        var kind = GetTblKind(gameId);

        foreach (var package in packages)
        {
            var tblPatchesDir = Path.Combine(package.Path, "tblpatches");
            var includePath = ApiCalls.ResolveArgs(package, "AddTblPatchesFolder", ArgKind.PackagePath);
            var patchesFiles = Directory.Exists(tblPatchesDir)
                ? Directory.EnumerateFiles(tblPatchesDir, "*.tbp", SearchOption.AllDirectories).ToList()
                : [];
            if (includePath is not null)
            {
                foreach (var path in includePath)
                    patchesFiles.AddRange(Directory.GetFiles(path, "*.tbp", SearchOption.AllDirectories));
            }

            foreach (var patchFile in patchesFiles)
            {
                var tablePatches = Yaml.TryLoad<TablePatches>(patchFile);
                if (tablePatches is null) continue;
                if (tablePatches.Version != 2)
                {
                    Logger.Error($"Invalid version for {patchFile}, skipping...");
                    continue;
                }

                foreach (var patch in tablePatches.Patches)
                {
                    if (!tables.Contains(patch.Tbl)) 
                    {
                        Logger.Error($"{patch.Tbl} doesn't exist in {gameId} required by {package.Metadata.Id}, skipping...");
                        continue;
                    }

                    // a bit hacky but for p5r switch if the table is NAME we just use the cpk lang
                    string relativePath;
                    if (gameId is "p5rswitch" && string.Equals(patch.Tbl, "NAME", StringComparison.OrdinalIgnoreCase))
                        relativePath = $"{cpkLang}/BATTLE/TABLE/{ResolveTblPath(patch.Tbl, gameId)}";
                    else
                        relativePath = Path.Combine(tblsPath, ResolveTblPath(patch.Tbl, gameId));

                    if (!GameFileService.CopyIfNotExist(relativePath, outputPath))
                    {
                        Logger.Error("TODO: Proper logging lol");
                        continue;
                    }
                }
            }
        }
    }
}
