using Novellus.Lib.Backend.Build.BinaryPatching;
using Novellus.Lib.Backend.Logging;
using System.Buffers.Binary;
using System.Text;

namespace Novellus.Lib.Backend.Build.TblPatching;

public static class TblConverter
{
    private static readonly Dictionary<string, string> TblNames = new()
    {
        { "SKL", "SKILL.TBL" },
        { "UNT", "UNIT.TBL" },
        { "MSG", "MSG.TBL" },
        { "PSA", "PERSONA.TBL" },
        { "ENC", "ENCOUNT.TBL" },
        { "EFF", "EFFECT.TBL" },
        { "MDL", "MODEL.TBL" },
        { "AIC", "AICALC.TBL" },
        { "AIF", "AICALC_F.TBL" },
        { "ENF", "ENCOUNT_F.TBL" },
        { "PSF", "PERSONA_F.TBL" },
        { "SKF", "SKILL_F.TBL" },
        { "UNF", "UNIT_F.TBL" },
        { "EAI", "ELSAI.TBL" },
        { "EXT", "EXIST.TBL" },
        { "ITM", "ITEM.TBL" },
        { "NME", "NAME.TBL" },
        { "PLY", "PLAYER.TBL" },
        { "TKI", "TALKINFO.TBL" },
        { "VSL", "VISUAL.TBL" },
    };
    public static BinaryPatch? Convert(string filePath, string gameId)
    {
        using FileStream fs = new(filePath, FileMode.Open);
        using BinaryReader reader = new(fs);

        if (fs.Length < 3)
        {
            Logger.Error("Improper .tblpatch format (what?)");
            return null;
        }

        string tblName = Encoding.ASCII.GetString(reader.ReadBytes(3));
        if (!TblNames.TryGetValue(tblName, out var fullTblName))
        {
            Logger.Error($"Error trying to convert tbl patch: '{filePath}': unknow tbl: {tblName}");
            return null;
        }
        long offset = BinaryPrimitives.ReadInt64BigEndian(reader.ReadBytes(8));

        if (fullTblName != "NAME.TBL")
        {
            byte[] fileContents = reader.ReadBytes((int)(fs.Length - fs.Position));
            string path = TblPatcher.GetTblsPath(gameId);
            BinaryPatch patch = new()
            {
                File = $"{path}/{fullTblName}",
                Offset = offset,
                Data = BitConverter.ToString(fileContents).Replace("-", " ")
            };
            return patch;
        }

        // TODO: handle NAME.TBL for persona 5/p5 royal
        if (fs.Length < 6)
        {
            Logger.Error("Improper .tblpatch format (what?)");
            return null;
        }

        return null;
    }
}
