using System;
using System.Drawing;

namespace QOI.NET.Chunk
{
    internal class Run8Reader : IChunkReader
    {
        public byte Tag => 0b010;
        public byte TagBitLength => 3;
        public byte Length => 1;

        public void WritePixels(Color[] pixels, ref int currentPixel, Span<byte> chunk)
        {
            // 5-bit run-length repeating the previous pixel: 1..32
            byte runLength  = (byte)(chunk[0] & 0b0001_1111); // erase tag bits
            runLength += 1;

            var color = pixels[currentPixel - 1];
            var run = pixels.AsSpan(currentPixel, runLength);
            for (int i = 0; i < run.Length; i++)
            {
                run[i] = color;
            }

            currentPixel += runLength;
        }
    }
}
