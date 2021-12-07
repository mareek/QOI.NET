using System;
using System.Drawing;
using System.IO;
using QOI.NET.Chunk;

namespace QOI.NET
{
    public class QoiDecoder
    {
        private readonly Run8Reader _run8Reader = new();
        private readonly ColorReader _colorReader = new();
        private readonly IndexReader _indexReader = new();

        public Bitmap Read(Stream stream)
        {
            var (width, height, _, _) = HeaderHelper.ReadHeader(stream);

            Color[] pixels = DecodePixels(stream, width, height);

            return CreateBitmap(width, height, pixels);
        }

        private Color[] DecodePixels(Stream stream, uint width, uint height)
        {
            Color[] pixels = new Color[(height * width)];
            Span<byte> chunkBuffer = stackalloc byte[5];
            var currentPixel = 0;
            while (currentPixel < pixels.Length)
            {
                stream.Read(chunkBuffer[0..1]);
                var chunkReader = ChunkReaderSelector(chunkBuffer[0]);
                if (chunkReader.Length > 1)
                {
                    stream.Read(chunkBuffer[1..chunkReader.Length]);
                }

                chunkReader.WritePixels(pixels, ref currentPixel, chunkBuffer[0..chunkReader.Length]);
                _indexReader.AddToIndex(pixels[currentPixel]);
                currentPixel += 1;
            }

            return pixels;
        }

        private IChunkReader ChunkReaderSelector(byte tagByte)
        {
            static bool CanReadChunk(IChunkReader chunkReader, byte tagByte)
                => (tagByte >> (8 - chunkReader.TagBitLength)) == chunkReader.Tag;

            if (CanReadChunk(_run8Reader, tagByte))
                return _run8Reader;
            if (CanReadChunk(_indexReader, tagByte))
                return _indexReader;
            if (CanReadChunk(_colorReader, tagByte))
                return _colorReader;

            throw new NotImplementedException();
        }

        private static Bitmap CreateBitmap(uint width, uint height, Color[] pixels)
        {
            var result = new Bitmap((int)width, (int)height);
            result.SetPixels(pixels);
            return result;
        }
    }
}
