using System;
using QOI.Core.Interface;

namespace QOI.Core.Chunk;

internal class RgbReader : IChunkReader
{
    public int ChunkLength => 4;

    public QoiColor WritePixels(IImageWriter imageWriter, ReadOnlySpan<byte> chunk, QoiColor previousPixel)
    {
        var pixel = QoiColor.FromArgb(previousPixel.A, chunk[1], chunk[2], chunk[3]);
        imageWriter.WritePixel(pixel.R, pixel.G, pixel.B, pixel.A);
        return pixel;
    }
}
