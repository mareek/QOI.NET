using System;

namespace QOI.Core.Chunk;

internal interface ISinglePixelChunkReader: IChunkReader
{
    QoiColor ReadPixel(ReadOnlySpan<byte> chunk, QoiColor previousPixel);
}
