using System.Collections.Generic;
using System.IO;

namespace QOI.Core.Chunk;

internal class RunWriter
{
    private const short MaxRunLength = 62;

    public bool CanHandlePixel(QoiColor currentPixel, QoiColor previousPixel) => currentPixel == previousPixel;

    public void WriteChunk(IEnumerator<QoiColor> pixelEnnumerator, QoiColor previousPixel, Stream stream, out bool endOfFile)
    {
        int runLength = 1;
        while (runLength < MaxRunLength
               && pixelEnnumerator.MoveNext()
               && pixelEnnumerator.Current == previousPixel)
        {
            runLength++;
        }

        // 6-bit run-length repeating the previous pixel: 1..62
        byte value = (byte)(runLength - 1);
        stream.WriteByte(Tag.RUN.Apply(value));

        endOfFile = runLength < MaxRunLength && pixelEnnumerator.Current == previousPixel;
    }
}
