using System;

namespace QOI.Core.Chunk;

internal class RgbaReader : IChunkReader
{
    public int ChunkLength => 5;

    public void WritePixels(QoiColor[] pixels, ref int currentPixel, ReadOnlySpan<byte> chunk)
    {
        pixels[currentPixel] = QoiColor.FromArgb(chunk[4], chunk[1], chunk[2], chunk[3]);
    }
}
