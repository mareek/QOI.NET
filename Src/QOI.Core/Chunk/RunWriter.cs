using System;
using System.IO;

namespace QOI.Core.Chunk;

internal class RunWriter
{
    private const short MaxRunLength = 62;

    public bool CanHandlePixel(QoiColor currentPixel, QoiColor previousPixel) => currentPixel == previousPixel;

    public void WriteChunk(ReadOnlySpan<QoiColor> pixels, int currentPixel, QoiColor previousPixel, Stream stream, out int runLength)
    {
        runLength = 0;
        while (runLength < MaxRunLength
               && (currentPixel + runLength) < pixels.Length
               && pixels[currentPixel + runLength] == previousPixel)
        {
            runLength++;
        }

        // 6-bit run-length repeating the previous pixel: 1..62
        byte value = (byte)(runLength - 1);
        stream.WriteByte(Tag.RUN.Apply(value));
    }
}
