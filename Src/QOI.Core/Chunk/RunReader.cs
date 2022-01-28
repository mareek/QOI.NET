using System;
using QOI.Core.Interface;

namespace QOI.Core.Chunk;

internal class RunReader : IChunkReader
{
    public int ChunkLength => 1;

    public QoiColor WritePixels(IImageWriter imageWriter, ReadOnlySpan<byte> chunk, QoiColor previousPixel)
    {
        int runLength = chunk[0] & 0b0011_1111; // erase tag bits

        // 6-bit run-length repeating the previous pixel: 1..62
        runLength += 1;

        for (int i = 0; i < runLength; i++)
        {
            imageWriter.WritePixel(previousPixel);
        }

        return previousPixel;
    }
}
