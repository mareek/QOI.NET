using System;
using System.Drawing;

namespace QOI.NET
{
    internal interface IChunkReader
    {
        byte Tag { get; }
        byte TagBitLength { get; }
        byte Length { get; }
        void WritePixels(Color[] pixels, ref int currentPixel, Span<byte> chunk);
    }
}
