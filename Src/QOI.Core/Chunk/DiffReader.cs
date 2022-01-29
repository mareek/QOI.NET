using System;
using QOI.Core.Interface;

namespace QOI.Core.Chunk;

internal class DiffReader : IChunkReader
{
    private const short MinDiff = -2;

    public int ChunkLength => 1;

    public QoiColor WritePixels(IImageWriter imageWriter, ReadOnlySpan<byte> chunk, QoiColor previousPixel)
    {
        var diff = ParseDiff(chunk[0]);
        var pixel = diff.GetPixel(previousPixel);
        imageWriter.WritePixel(pixel);
        return pixel;
    }

    private QoiColorDiff ParseDiff(byte chunk)
    {
        // 2-bit tag b01
        // 2-bit red channel difference from the previous pixel -2..1
        // 2-bit green channel difference from the previous pixel -2..1
        // 2-bit blue channel difference from the previous pixel -2..1

        int rDiff = TruncateToBits((chunk >> 4), 2) + MinDiff;
        int gDiff = TruncateToBits((chunk >> 2), 2) + MinDiff;
        int bDiff = TruncateToBits(chunk, 2) + MinDiff;
        return new(0, (sbyte)rDiff, (sbyte)gDiff, (sbyte)bDiff);
    }

    private static int TruncateToBits(int number, byte bitCount)
        => number & ((1 << bitCount) - 1);
}
