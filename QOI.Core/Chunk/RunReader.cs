using System;

namespace QOI.Core.Chunk;

internal class RunReader : IChunkReader
{
    public bool CanReadChunk(byte tagByte, out int chunkLength)
    {
        chunkLength = 1;
        return Tag.RUN.IsPresent(tagByte);
    }

    public void WritePixels(QoiColor[] pixels, ref int currentPixel, Span<byte> chunk)
    {
        int runLength = chunk[0] & 0b0011_1111; // erase tag bits

        // 6-bit run-length repeating the previous pixel: 1..62
        runLength += 1;

        var color = pixels[currentPixel - 1];
        var run = pixels.AsSpan(currentPixel, runLength);
        for (int i = 0; i < run.Length; i++)
        {
            run[i] = color;
        }

        currentPixel += runLength - 1;
    }
}
