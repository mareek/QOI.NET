using System;
using System.Drawing;
using System.IO;

namespace QOI.NET
{
    internal class ColorChunkWriter : IChunkWriter
    {
        public bool CanHandlePixel(Color[] pixels, int currentPixel) => true;

        public void WriteChunk(Color[] pixels, ref int currentPixel, Stream stream)
        {
            var pixel = pixels[currentPixel++];

            Span<byte> buffer = stackalloc byte[5];

            buffer[0] = 0b1111_1111;
            buffer[1] = pixel.R;
            buffer[2] = pixel.G;
            buffer[3] = pixel.B;
            buffer[4] = pixel.A;

            stream.Write(buffer);
        }
    }
}
