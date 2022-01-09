using System;
using QOI.Core.Interface;

namespace QOI.Core.Chunk;

internal class RgbaReader : IChunkReader
{
    public int ChunkLength => 5;

    public QoiColor WritePixels(IImageWriter imageWriter, ReadOnlySpan<byte> chunk, QoiColor previousPixel)
    {
        var pixel = QoiColor.FromArgb(chunk[4], chunk[1], chunk[2], chunk[3]);
        imageWriter.WritePixel(pixel.R, pixel.G, pixel.B, pixel.A);
        return pixel;
    }
}
