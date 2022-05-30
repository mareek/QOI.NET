using System;

namespace QOI.Core.Chunk;

internal class RunReader : IChunkReader
{
    public int ChunkLength => 1;

    public int GetRunLength(ReadOnlySpan<byte> chunk)
    {
        int runLength = chunk[0] & 0b0011_1111; // erase tag bits

        // 6-bit run-length repeating the previous pixel: 1..62
        return runLength + 1;
    }
}
