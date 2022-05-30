using System;

namespace QOI.Core.Chunk;

internal class LumaReader : ISinglePixelChunkReader
{
    public int ChunkLength => 2;

    public QoiColor ReadPixel(ReadOnlySpan<byte> chunk, QoiColor previousPixel)
    {
        QoiColorDiff diff = ReadDiff(chunk);
        return diff.GetPixel(previousPixel);
    }

    private QoiColorDiff ReadDiff(ReadOnlySpan<byte> chunk)
    {
        // 6-bit green channel difference from the previous pixel -32..31
        sbyte gDiff = (sbyte)(Tag.LUMA.Erase(chunk[0]) - 32);
        // 4-bit red channel difference minus green channel difference -8..7
        sbyte rDiff = (sbyte)(gDiff + (chunk[1] >> 4) - 8);
        // 4-bit blue channel difference minus green channel difference -8..7
        sbyte bDiff = (sbyte)(gDiff + (chunk[1] & 0b1111) - 8);
        return new QoiColorDiff(0, rDiff, gDiff, bDiff);
    }
}
