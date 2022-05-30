using System;

namespace QOI.Core.Chunk;

internal class IndexReader : ISinglePixelChunkReader
{
    private readonly PixelIndex _pixelIndex = new();

    public void AddToIndex(QoiColor pixel) => _pixelIndex.Add(pixel);

    public int ChunkLength => 1;

    public QoiColor ReadPixel(ReadOnlySpan<byte> chunk, QoiColor previousPixel) => _pixelIndex.Get(chunk[0]);
}
