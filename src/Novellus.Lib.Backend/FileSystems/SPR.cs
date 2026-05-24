using Novellus.Lib.Backend.Logging;
using System.Buffers;
using System.Text;

namespace Novellus.Lib.Backend.FileSystems;


public static class SPR
{
    private static int Search(byte[] src, byte[] pattern)
    {
        int c = src.Length - pattern.Length + 1;
        int j;
        for (int i = 0; i < c; i++)
        {
            if (src[i] != pattern[0]) continue;
            for (j = pattern.Length - 1; j >= 1 && src[i + j] == pattern[j]; j--) ;
            if (j == 0) return i;
        }
        return -1;
    }
    private static string GetTmxName(byte[] tmx)
    {
        int end = Search(tmx, [0x00]);
        byte[] name = tmx.Take(end).ToArray();
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        // 932 = Shift_JIS, the encoding used for tmx names
        return Encoding.GetEncoding(932).GetString(name);
    }
    private static void UpdateOffsets(MemoryStream spr, List<int> offsets)
    {
        // Start of tmx offsets
        int pos = 36;

        foreach (int offset in offsets)
        {
            byte[] offsetBytes = BitConverter.GetBytes(offset);
            spr.Position = pos;
            spr.Write(offsetBytes, 0, 4);
            pos += 8;
        }
    }
    private static List<int> GetTmxOffsets(MemoryStream spr)
    {
        byte[] sprBytes = spr.GetBuffer();
        byte[] pattern = Encoding.ASCII.GetBytes("TMX0");
        List<int> tmxOffsets = [];

        int offset = 0;
        int found = 0;
        while (found != -1)
        {
            // Start search after "TMX0"
            found = Search(sprBytes[offset..], pattern);
            offset = found + offset + 4;
            if (found != -1)
            {
                tmxOffsets.Add(offset - 12);
            }
        }

        return tmxOffsets;
    }

    public static Dictionary<string, int> GetTmxNames(MemoryStream spr)
    {
        Dictionary<string, int> tmxNames = [];

        byte[] sprBytes = spr.GetBuffer();
        byte[] pattern = Encoding.ASCII.GetBytes("TMX0");
        int offset = 0;
        int found = 0;

        while (found != -1)
        {
            // Start search after "TMX0"
            found = Search(sprBytes[offset..], pattern);
            offset = found + offset + 4;
            if (found != -1)
            {
                string ogTmxName = GetTmxName(sprBytes[(offset + 24)..]);
                string tmxName = ogTmxName;
                int index = 2;
                while (tmxNames.ContainsKey(tmxName))
                {
                    tmxName = $"{ogTmxName}({index})";
                    index += 1;
                }
                tmxNames.Add(tmxName, offset - 12);
            }

        }

        return tmxNames;
    }

    public static MemoryStream MergeSpr(Stream ogSprStream, List<(string TmxName, string SourcePath)> files, string sprName)
    {
        // we're going to use a memory stream to avoid writing/reading disk
        var sprStream = new MemoryStream();
        ogSprStream.CopyTo(sprStream);
        sprStream.Position = 0;
        ogSprStream.Position = 0;

        // name -> offset
        var sprTxmNames = GetTmxNames(sprStream);

        // small buffer for reading tmx lengths
        Span<byte> buffer = stackalloc byte[4];

        foreach (var (TmxName, SourcePath) in files)
        {
            var tmxName = Path.GetFileNameWithoutExtension(TmxName);
            if (!sprTxmNames.TryGetValue(tmxName, out int tmxOffset))
            {
                Logger.Warn($"Couldn't find '{TmxName}' in '{sprName}'. Skipping...");
                continue;
            }
            Logger.Info($"Merging '{tmxName}' onto '{sprName}'");

            using var txmStream = File.OpenRead(SourcePath);
            int repTmxLen = (int)txmStream.Length;

            sprStream.Position = tmxOffset + 4;
            sprStream.Read(buffer);
            int ogTmxLen = BitConverter.ToInt32(buffer);

            // early continue if the lengths are the same, we can just overwrite the tmx
            if (repTmxLen == ogTmxLen)
            {
                sprStream.Position = tmxOffset;
                txmStream.CopyTo(sprStream);
                continue;
            }

            // if the lengths are different, we need to shift the rest of the file
            int tailStart = tmxOffset + ogTmxLen;
            int tailLen = (int)(sprStream.Length - tailStart);

            byte[] tail = ArrayPool<byte>.Shared.Rent(tailLen);
            try
            {
                sprStream.Position = tailStart;
                sprStream.ReadExactly(tail, 0, tailLen);

                sprStream.SetLength(tmxOffset);
                sprStream.Position = tmxOffset;
                txmStream.CopyTo(sprStream);
                sprStream.Write(tail, 0, tailLen);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(tail);
            }
            sprStream.Position = 0;
            UpdateOffsets(sprStream, GetTmxOffsets(sprStream));
            sprStream.Position = 0;
            sprTxmNames = GetTmxNames(sprStream);
        }

        sprStream.Position = 0;
        return sprStream;
    }
}