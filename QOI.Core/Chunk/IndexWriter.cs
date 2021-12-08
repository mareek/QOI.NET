using System;
using System.IO;

namespace QOI.Core.Chunk;

internal class IndexWriter : IChunkWriter
{
    private readonly PixelIndex _pixelIndex = new();

    public void AddToIndex(QoiColor pixel) => _pixelIndex.Add(pixel);

    public bool CanHandlePixel(ReadOnlySpan<QoiColor> pixels, int currentPixel) => _pixelIndex.Exists(pixels[currentPixel]);

    public void WriteChunk(ReadOnlySpan<QoiColor> pixels, ref int currentPixel, Stream stream)
    {
        var index = _pixelIndex.GetIndex(pixels[currentPixel]);
        stream.WriteByte((byte)index);
    }
}
