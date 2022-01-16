using System;
using System.IO;

namespace QOI.Core.Chunk;

internal class RgbaWriter
{
    public void WriteChunk(QoiColor pixel, Stream stream)
    {
        Span<byte> buffer = stackalloc byte[5];

        buffer[1] = pixel.R;
        buffer[2] = pixel.G;
        buffer[3] = pixel.B;
        buffer[4] = pixel.A;

        Tag.RGBA.Write(buffer);
        stream.Write(buffer);
    }
}
