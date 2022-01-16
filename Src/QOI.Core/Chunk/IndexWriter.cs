using System.IO;

namespace QOI.Core.Chunk;

internal class IndexWriter
{
    private readonly PixelIndex _pixelIndex = new();

    public void AddToIndex(QoiColor pixel) => _pixelIndex.Add(pixel);

    public bool CanHandlePixel(QoiColor currentPixel) => _pixelIndex.Exists(currentPixel);

    public void WriteChunk(QoiColor currentPixel, Stream stream)
    {
        var index = (byte)_pixelIndex.GetIndex(currentPixel);
        stream.WriteByte(Tag.INDEX.Apply(index));
    }
}
