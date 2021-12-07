using System.Drawing;
using System.IO;

namespace QOI.NET.Chunk
{
    internal class IndexWriter : IChunkWriter
    {
        private readonly PixelIndex _pixelIndex = new();

        public void AddToIndex(Color pixel) => _pixelIndex.Add(pixel);

        public bool CanHandlePixel(Color[] pixels, int currentPixel) => _pixelIndex.Exists(pixels[currentPixel]);

        public void WriteChunk(Color[] pixels, ref int currentPixel, Stream stream)
        {
            var index = _pixelIndex.GetIndex(pixels[currentPixel]);
            stream.WriteByte((byte)index);
        }
    }
}
