using System;
using System.Buffers.Binary;
using System.IO;

namespace QOI.Core.Chunk;

internal class RunWriter
{
    private const short MaxRun8Length = 32;
    private const short MaxRun16Length = 8224;

    public bool CanHandlePixel(QoiColor currentPixel, QoiColor previousPixel) => currentPixel == previousPixel;

    public void WriteChunk(ReadOnlySpan<QoiColor> pixels, int currentPixel, QoiColor previousPixel, Stream stream, out int runLength)
    {
        runLength = 0;
        while (runLength < MaxRun16Length
               && (currentPixel + runLength) < pixels.Length
               && pixels[currentPixel + runLength] == previousPixel)
        {
            runLength++;
        }

        if (runLength <= MaxRun8Length)
        {
            // 5-bit run-length repeating the previous pixel: 1..32
            byte value = (byte)(runLength - 1);
            stream.WriteByte(value.WithTag(Tag.Run8, 3));
        }
        else
        {
            // 13-bit run-length repeating the previous pixel: 33..8224
            Span<byte> chunk = stackalloc byte[2];
            BinaryPrimitives.WriteInt16BigEndian(chunk, (short)(runLength - 33));
            chunk.WriteTag(Tag.Run16, 3);
            stream.Write(chunk);
        }
    }
}
