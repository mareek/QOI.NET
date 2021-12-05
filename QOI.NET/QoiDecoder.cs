using System;
using System.Buffers.Binary;
using System.Drawing;
using System.IO;

namespace QOI.NET
{
    public class QoiDecoder
    {
        private readonly ColorChunkReader _colorChunkReader = new();

        public Bitmap Read(Stream stream)
        {
            Span<byte> header = stackalloc byte[FromatConstants.HeaderLength];
            stream.Read(header);

            if (!header[0..4].SequenceEqual(FromatConstants.MagicBytes))
                throw new FormatException("This is not a valid QOI image");

            var width = BinaryPrimitives.ReadUInt32BigEndian(header[4..8]);
            var height = BinaryPrimitives.ReadUInt32BigEndian(header[8..12]);
            var hasAlphaChannel = header[12] == 4;
            Color[] pixels = DecodePixels(stream, width, height);

            return CreateBitmap(width, height, pixels);
        }

        private Color[] DecodePixels(Stream stream, uint width, uint height)
        {
            uint pixelCount = height * width;
            Color[] pixels = new Color[pixelCount];
            Span<byte> chunkBuffer = stackalloc byte[5];
            var currentPixel = 0;
            while (currentPixel < pixelCount)
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
            if ((tagByte >> 4) == 0b1111)
            {
                return _colorChunkReader;
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
