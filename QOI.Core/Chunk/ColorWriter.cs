using System;
using System.IO;

namespace QOI.Core.Chunk;

internal class ColorWriter : IChunkWriter
{
    public bool CanHandlePixel(ReadOnlySpan<QoiColor> pixels, int currentPixel) => true;

    public void WriteChunk(ReadOnlySpan<QoiColor> pixels, ref int currentPixel, Stream stream)
    {
        var pixel = pixels[currentPixel];

        Span<byte> buffer = stackalloc byte[5];

        buffer[0] = 0b1111_1111;
        buffer[1] = pixel.R;
        buffer[2] = pixel.G;
        buffer[3] = pixel.B;
        buffer[4] = pixel.A;

        stream.Write(buffer);
    }
}
