using System;

namespace QOI.Core.Chunk;

internal class IndexReader : IChunkReader
{
    private readonly PixelIndex _pixelIndex = new();

    public void AddToIndex(QoiColor pixel) => _pixelIndex.Add(pixel);

    public int ChunkLength => 1;

    public void WritePixels(QoiColor[] pixels, ref int currentPixel, ReadOnlySpan<byte> chunk)
    {
        pixels[currentPixel] = _pixelIndex.Get(chunk[0]);
    }
}
