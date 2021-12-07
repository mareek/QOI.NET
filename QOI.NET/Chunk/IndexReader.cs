using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace QOI.NET.Chunk
{
    internal class IndexReader : IChunkReader
    {
        private readonly PixelIndex _pixelIndex = new();

        public void AddToIndex(Color pixel) => _pixelIndex.Add(pixel);

        public byte Tag => 0b00;

        public byte TagBitLength => 2;

        public byte Length => 1;

        public void WritePixels(Color[] pixels, ref int currentPixel, Span<byte> chunk)
        {
            pixels[currentPixel] = _pixelIndex.Get(chunk[0]);
        }
    }
}
