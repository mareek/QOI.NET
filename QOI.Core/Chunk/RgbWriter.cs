using System;
using System.IO;

namespace QOI.Core.Chunk;

internal class RgbWriter
{
    public bool CanHandlePixel(QoiColor currentPixel) => currentPixel.A == 255;

    public void WriteChunk(QoiColor pixel, Stream stream)
    {
        Span<byte> buffer = stackalloc byte[4];

        buffer[1] = pixel.R;
        buffer[2] = pixel.G;
        buffer[3] = pixel.B;

        Tag.RGB.Write(buffer);
        stream.Write(buffer);
    }
}
