using System;

namespace QOI.Core.Chunk;

internal class RgbReader : ISinglePixelChunkReader
{
    public int ChunkLength => 4;

    public QoiColor ReadPixel(ReadOnlySpan<byte> chunk, QoiColor previousPixel)
        => QoiColor.FromArgb(previousPixel.A, chunk[1], chunk[2], chunk[3]);
}
