using System;
using System.Buffers.Binary;
using System.Drawing;
using System.IO;
using System.Linq;
using QOI.NET.Chunk;

namespace QOI.NET
{
    public class QoiEncoder
    {
        private readonly IChunkWriter[] _chunkWriters = { new Run8Writer(), new ColorWriter() };

        public byte[] Write(Bitmap image)
        {
            using var stream = new MemoryStream();

            WriteHeader(image, stream);
            EncodePixels(image.GetPixels().ToArray(), stream);

            return stream.ToArray();
        }

        private static void WriteHeader(Bitmap image, MemoryStream stream)
        {
            HeaderHelper.WriteHeader(stream, (uint)image.Width, (uint)image.Height);
        }

        private void EncodePixels(Color[] pixels, Stream stream)
        {
            var currentPixel = 0;
            while (currentPixel < pixels.Length)
            {
                var chunkWriter = ChunkWriterSelector(pixels, currentPixel);
                chunkWriter.WriteChunk(pixels, ref currentPixel, stream);
            }
        }

        private IChunkWriter ChunkWriterSelector(Color[] pixels, int currentPixel)
        {
            foreach (var chunkWriter in _chunkWriters)
            {
                if (chunkWriter.CanHandlePixel(pixels, currentPixel))
                {
                    return chunkWriter;
                }
            }

            throw new NotImplementedException();
        }
    }
}
