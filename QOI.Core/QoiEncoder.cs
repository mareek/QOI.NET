using System;
using System.IO;
using QOI.Core.Chunk;

namespace QOI.Core;

public class QoiEncoder
{
    private readonly RunWriter _run8Writer = new();
    private readonly ColorWriter _colorWriter = new();
    private readonly IndexWriter _indexWriter = new();

    public void Write(QoiImage image, Stream stream)
    {
        HeaderHelper.WriteHeader(stream, image.Width, image.Height, image.HasAlpha, image.ColorSpace);
        EncodePixels(image.Pixels, stream);
    }

    private void EncodePixels(ReadOnlySpan<QoiColor> pixels, Stream stream)
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

    private IChunkWriter ChunkWriterSelector(ReadOnlySpan<QoiColor> pixels, int currentPixel)
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
