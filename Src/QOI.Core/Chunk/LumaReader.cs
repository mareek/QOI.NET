using System;
using QOI.Core.Interface;

namespace QOI.Core.Chunk;

internal class LumaReader : IChunkReader
{
    public int ChunkLength => 2;

    public QoiColor WritePixels(IImageWriter imageWriter, ReadOnlySpan<byte> chunk, QoiColor previousPixel)
    {
        QoiColorDiff diff = ReadDiff(chunk);
        var pixel = diff.GetPixel(previousPixel);
        imageWriter.WritePixel(pixel.R, pixel.G, pixel.B, pixel.A);
        return pixel;
    }

    private QoiColorDiff ReadDiff(ReadOnlySpan<byte> chunk)
    {
        // 6-bit green channel difference from the previous pixel -32..31
        short gDiff = (short)(Tag.LUMA.Erase(chunk[0]) - 32);
        // 4-bit red channel difference minus green channel difference -8..7
        short rDiff = (short)(gDiff + (chunk[1] >> 4) - 8);
        // 4-bit blue channel difference minus green channel difference -8..7
        short bDiff = (short)(gDiff + (chunk[1] & 0b1111) - 8);
        return new QoiColorDiff(0, rDiff, gDiff, bDiff);
    }
}
