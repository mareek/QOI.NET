using System;
using System.IO;

namespace QOI.Core.Chunk;

internal interface IChunkWriter
{
    bool CanHandlePixel(ReadOnlySpan<QoiColor> pixels, int currentPixel);
    void WriteChunk(ReadOnlySpan<QoiColor> pixels, ref int currentPixel, Stream stream);
}
