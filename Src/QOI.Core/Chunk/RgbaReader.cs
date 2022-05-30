using System;

namespace QOI.Core.Chunk;

internal class RgbaReader : ISinglePixelChunkReader
{
    public int ChunkLength => 5;

    public QoiColor ReadPixel(ReadOnlySpan<byte> chunk, QoiColor previousPixel) 
        => QoiColor.FromArgb(chunk[4], chunk[1], chunk[2], chunk[3]);
}
