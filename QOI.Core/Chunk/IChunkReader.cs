using System;

namespace QOI.Core.Chunk;

internal interface IChunkReader
{
    int ChunkLength { get; }
    void WritePixels(QoiColor[] pixels, ref int currentPixel, ReadOnlySpan<byte> chunk);
}
