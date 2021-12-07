using System;
using System.Drawing;

namespace QOI.NET.Chunk
{
    internal class ColorReader : IChunkReader
    {
        public byte Tag => 0b1111;
        public byte TagBitLength => 4;
        public byte Length => 5;

        public void WritePixels(Color[] pixels, ref int currentPixel, Span<byte> chunk)
        {
            pixels[currentPixel] = Color.FromArgb(chunk[4], chunk[1], chunk[2], chunk[3]);
        }
    }
}
