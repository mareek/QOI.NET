using System;

namespace QOI.Core.Chunk;

internal class RgbaReader : IChunkReader
{
    public bool CanReadChunk(byte tagByte, out int chunkLength)
    {
        chunkLength = 5;
        return Tag.RGBA.IsPresent(tagByte);
    }

    public void WritePixels(QoiColor[] pixels, ref int currentPixel, ReadOnlySpan<byte> chunk)
    {
        pixels[currentPixel] = QoiColor.FromArgb(chunk[4], chunk[1], chunk[2], chunk[3]);
    }
}
