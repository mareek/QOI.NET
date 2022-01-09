using System;

namespace QOI.Core.Chunk
{
    internal class LumaReader : IChunkReader
    {
        public int ChunkLength => 2;

        public void WritePixels(QoiColor[] pixels, ref int currentPixel, ReadOnlySpan<byte> chunk)
        {
            var previousPixel = ChunkHelper.GetPreviousPixel(pixels, currentPixel);
            QoiColorDiff diff = ReadDiff(chunk);
            pixels[currentPixel] = diff.GetPixel(previousPixel);
        }

        private QoiColorDiff ReadDiff(ReadOnlySpan<byte> chunk)
        {
            // 6-bit green channel difference from the previous pixel -32..31
            short gDiff = (short)(Tag.LUMA.Erase(chunk[0]) - 32);
            // 4-bit red channel difference minus green channel difference -8..7
            short rDiff = (short)(gDiff + (chunk[1] >> 4) - 8);
            // 4-bit blue channel difference minus green channel difference -8..7
            short bDiff = (short)(gDiff + (chunk[1] & 0b1111) - 8);
            return new QoiColorDiff(rDiff, gDiff, bDiff);
        }
    }
}
