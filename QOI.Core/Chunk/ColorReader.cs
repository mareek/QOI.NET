using System;

namespace QOI.Core.Chunk;

internal class ColorReader : IChunkReader
{
    public bool CanReadChunk(byte tagByte, out int chunkLength)
    {
        chunkLength = 5;
        return (tagByte >> 4) == 0b1111;
    }

    public void WritePixels(QoiColor[] pixels, ref int currentPixel, Span<byte> chunk)
    {
        pixels[currentPixel] = QoiColor.FromArgb(chunk[4], chunk[1], chunk[2], chunk[3]);
    }
}
