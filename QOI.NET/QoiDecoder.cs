using System;
using System.Drawing;
using System.IO;
using QOI.NET.Chunk;

namespace QOI.NET
{
    public class QoiDecoder
    {
        private readonly IChunkReader[] _chunkReaders = { new Run8Reader(), new ColorReader() };

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
            }

            return pixels;
        }

        private IChunkReader ChunkReaderSelector(byte tagByte)
        {
            static bool CanReadChunk(IChunkReader chunkReader, byte tagByte)
                => (tagByte >> (8 - chunkReader.TagBitLength)) == chunkReader.Tag;

            foreach (var chunkReader in _chunkReaders)
            {
                if (CanReadChunk(chunkReader, tagByte))
                {
                    return chunkReader;
                }
            }

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
