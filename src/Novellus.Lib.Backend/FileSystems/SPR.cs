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
        var sprStreamMemory = new MemoryStream();
        ogSprStream.CopyTo(sprStreamMemory);
        sprStreamMemory.Position = 0;
        ogSprStream.Position = 0;

        // name -> offset
        var sprTxmNames = GetTmxNames(sprStreamMemory);

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

            // if we have a lot of textures it's better to use ArrayPool to avoid large allocations on the LOH
            var fileInfo = new FileInfo(SourcePath);
            int repTmxLen = (int)fileInfo.Length;
            byte[] tmxBytes = ArrayPool<byte>.Shared.Rent(repTmxLen);

            try
            {
                using (var stream = File.OpenRead(SourcePath))
                    stream.ReadExactly(tmxBytes, 0, repTmxLen);

                sprStreamMemory.Position = tmxOffset + 4;
                sprStreamMemory.Read(buffer);
                int ogTmxLen = BitConverter.ToInt32(buffer);

                if (repTmxLen == ogTmxLen)
                {
                    sprStreamMemory.Position = tmxOffset;
                    sprStreamMemory.Write(tmxBytes, 0, repTmxLen);
                }
                else
                {
                    int oldSize = (int)sprStreamMemory.Length;
                    int newSize = oldSize + (repTmxLen - ogTmxLen);
                    byte[] newSpr = ArrayPool<byte>.Shared.Rent(newSize);

                    try
                    {
                        sprStreamMemory.Position = 0;
                        sprStreamMemory.Read(newSpr, 0, tmxOffset);

                        tmxBytes.AsSpan(0, repTmxLen).CopyTo(newSpr.AsSpan(tmxOffset));

                        int tailStart = tmxOffset + ogTmxLen;
                        int tailLen = oldSize - tailStart;
                        sprStreamMemory.Position = tailStart;
                        sprStreamMemory.Read(newSpr, tmxOffset + repTmxLen, tailLen);

                        sprStreamMemory.SetLength(0);
                        sprStreamMemory.Write(newSpr, 0, newSize);
                    }
                    finally
                    {
                        ArrayPool<byte>.Shared.Return(newSpr);
                    }
                    sprStreamMemory.Position = 0;
                    UpdateOffsets(sprStreamMemory, GetTmxOffsets(sprStreamMemory));
                    sprStreamMemory.Position = 0;
                    sprTxmNames = GetTmxNames(sprStreamMemory);
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(tmxBytes);
            }
        }

        sprStreamMemory.Position = 0;
        return sprStreamMemory;
    }
}