using System;

namespace QOI.Core.Chunk;

internal class RgbReader : IChunkReader
{
    public int ChunkLength => 4;

    public void WritePixels(QoiColor[] pixels, ref int currentPixel, ReadOnlySpan<byte> chunk)
    {
        var previousPixel = ChunkHelper.GetPreviousPixel(pixels, currentPixel);
        pixels[currentPixel] = QoiColor.FromArgb(previousPixel.A, chunk[1], chunk[2], chunk[3]);
    }
}
