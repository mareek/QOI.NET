using System;
using System.IO;

namespace QOI.Core.Chunk
{
    internal class RunWriter : IChunkWriter
    {
        private const int MaxRunLength = 32;

        public bool CanHandlePixel(ReadOnlySpan<QoiColor> pixels, int currentPixel)
            => currentPixel > 0 && pixels[currentPixel - 1] == pixels[currentPixel];

        public void WriteChunk(ReadOnlySpan<QoiColor> pixels, ref int currentPixel, Stream stream)
        {
            var previousPixel = pixels[currentPixel - 1];
            int runLength = 0;
            while (runLength < MaxRunLength
                   && (currentPixel + runLength) < pixels.Length
                   && pixels[currentPixel + runLength] == previousPixel)
            {
                runLength++;
            }

            // 5-bit run-length repeating the previous pixel: 1..32
            stream.WriteByte((byte)(0b0100_0000 | (runLength - 1)));

            currentPixel += runLength - 1;
        }
    }
}
