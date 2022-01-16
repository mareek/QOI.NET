using System;
using QOI.Core.Interface;

namespace QOI.Core.Chunk;

internal class IndexReader : IChunkReader
{
    private readonly PixelIndex _pixelIndex = new();

    public void AddToIndex(QoiColor pixel) => _pixelIndex.Add(pixel);

    public int ChunkLength => 1;

    public QoiColor WritePixels(IImageWriter imageWriter, ReadOnlySpan<byte> chunk, QoiColor previousPixel)
    {
        var pixel = _pixelIndex.Get(chunk[0]);
        imageWriter.WritePixel(pixel.R, pixel.G, pixel.B, pixel.A);
        return pixel;
    }
}
