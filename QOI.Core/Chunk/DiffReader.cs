using System;

namespace QOI.Core.Chunk;

internal class DiffReader : IChunkReader
{
    public int ChunkLength => 1;

    public void WritePixels(QoiColor[] pixels, ref int currentPixel, ReadOnlySpan<byte> chunk)
    {
        var diff = ParseDiff(chunk[0]);
        var previousPixel = ChunkHelper.GetPreviousPixel(pixels, currentPixel);
        pixels[currentPixel] = diff.GetPixel(previousPixel);
    }

    private QoiColorDiff ParseDiff(byte chunk)
    {
        // 2-bit tag b01
        // 2-bit red channel difference from the previous pixel -2..1
        // 2-bit green channel difference from the previous pixel -2..1
        // 2-bit blue channel difference from the previous pixel -2..1

        int rDiff = TruncateToBits((chunk >> 4), 2) + ChunkHelper.MinDiff;
        int gDiff = TruncateToBits((chunk >> 2), 2) + ChunkHelper.MinDiff;
        int bDiff = TruncateToBits(chunk, 2) + ChunkHelper.MinDiff;
        return new((short)rDiff, (short)gDiff, (short)bDiff);
    }

    private static int TruncateToBits(int number, byte bitCount)
        => number & ((1 << bitCount) - 1);
}
