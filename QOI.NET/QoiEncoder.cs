using System;
using System.Drawing;
using System.IO;
using System.Linq;
using QOI.NET.Chunk;

namespace QOI.NET
{
    public class QoiEncoder
    {
        private readonly Run8Writer _run8Writer = new();
        private readonly ColorWriter _colorWriter = new();
        private readonly IndexWriter _indexWriter = new();

        public byte[] Write(Bitmap image)
        {
            using var stream = new MemoryStream();

            HeaderHelper.WriteHeader(stream, (uint)image.Width, (uint)image.Height);
            EncodePixels(image.GetPixels().ToArray(), stream);

            return stream.ToArray();
        }

        private void EncodePixels(Color[] pixels, Stream stream)
        {
            var currentPixel = 0;
            while (currentPixel < pixels.Length)
            {
                var chunkWriter = ChunkWriterSelector(pixels, currentPixel);
                chunkWriter.WriteChunk(pixels, ref currentPixel, stream);
                _indexWriter.AddToIndex(pixels[currentPixel]);
                currentPixel += 1;
            }
        }

        private IChunkWriter ChunkWriterSelector(Color[] pixels, int currentPixel)
        {
            if (_run8Writer.CanHandlePixel(pixels, currentPixel))
                return _run8Writer;
            if (_indexWriter.CanHandlePixel(pixels, currentPixel))
                return _indexWriter;
            if (_colorWriter.CanHandlePixel(pixels, currentPixel))
                return _colorWriter;

            throw new NotImplementedException();
        }
    }
}
