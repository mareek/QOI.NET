using System;
using QOI.Core.Interface;

namespace QOI.Core.Chunk;

internal interface IChunkReader
{
    int ChunkLength { get; }
    QoiColor WritePixels(IImageWriter imageWriter, ReadOnlySpan<byte> chunk, QoiColor previousPixel);
}
