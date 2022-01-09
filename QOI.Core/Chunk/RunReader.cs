using System;

namespace QOI.Core.Chunk;

internal class RunReader : IChunkReader
{
    public int ChunkLength => 1;

    public void WritePixels(QoiColor[] pixels, ref int currentPixel, ReadOnlySpan<byte> chunk)
    {
        int runLength = chunk[0] & 0b0011_1111; // erase tag bits

        // 6-bit run-length repeating the previous pixel: 1..62
        runLength += 1;

        var color = ChunkHelper.GetPreviousPixel(pixels, currentPixel);
        var run = pixels.AsSpan(currentPixel, runLength);
        for (int i = 0; i < run.Length; i++)
        {
            run[i] = color;
        }

        currentPixel += runLength - 1;
    }
}
