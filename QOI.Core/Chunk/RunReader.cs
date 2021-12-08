using System;

namespace QOI.Core.Chunk;

internal class RunReader : IChunkReader
{
    public bool CanReadChunk(byte tagByte, out int chunkLength)
    {
        if ((tagByte >> 5) == 0b011)
        {
            chunkLength = 2;
            return true;
        }
        chunkLength = 1;
        return (tagByte >> 5) == 0b010;
    }

    public void WritePixels(QoiColor[] pixels, ref int currentPixel, Span<byte> chunk)
    {
        int runLength = chunk[0] & 0b0001_1111; // erase tag bits
        if (chunk.Length == 2)
        {
            // 13-bit run-length repeating the previous pixel: 33..8224
            runLength = runLength * 256 + chunk[1] + 33;
        }
        else
        {
            // 5-bit run-length repeating the previous pixel: 1..32
            runLength += 1;
        }

        var color = pixels[currentPixel - 1];
        var run = pixels.AsSpan(currentPixel, runLength);
        for (int i = 0; i < run.Length; i++)
        {
            run[i] = color;
        }

        currentPixel += runLength - 1;
    }
}
