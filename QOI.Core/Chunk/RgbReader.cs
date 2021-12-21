using System;

namespace QOI.Core.Chunk;

internal class RgbReader : IChunkReader
{
    public bool CanReadChunk(byte tagByte, out int chunkLength)
    {
        chunkLength = 4;
        return Tag.RGB.IsPresent(tagByte);
    }

    public void WritePixels(QoiColor[] pixels, ref int currentPixel, ReadOnlySpan<byte> chunk)
    {
        pixels[currentPixel] = QoiColor.FromRgb(chunk[1], chunk[2], chunk[3]);
    }
}
