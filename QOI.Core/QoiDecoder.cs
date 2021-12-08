using System;
using System.IO;
using QOI.Core.Chunk;

namespace QOI.Core;

public class QoiDecoder
{
    private readonly RunReader _runReader = new();
    private readonly ColorReader _colorReader = new();
    private readonly IndexReader _indexReader = new();

    public QoiImage Read(Stream stream)
    {
        var (width, height, hasAlpha, colorSpace) = HeaderHelper.ReadHeader(stream);

        QoiColor[] pixels = DecodePixels(stream, width, height);

        return new QoiImage(width, height, hasAlpha, colorSpace, pixels);
    }

    private QoiColor[] DecodePixels(Stream stream, uint width, uint height)
    {
        QoiColor[] pixels = new QoiColor[(height * width)];
        Span<byte> chunkBuffer = stackalloc byte[5];
        var currentPixel = 0;
        while (currentPixel < pixels.Length)
        {
            stream.Read(chunkBuffer[0..1]);
            var chunkReader = ChunkReaderSelector(chunkBuffer[0], out int chunkLength);
            if (chunkLength > 1)
            {
                stream.Read(chunkBuffer[1..chunkLength]);
            }

            chunkReader.WritePixels(pixels, ref currentPixel, chunkBuffer[0..chunkLength]);

            _indexReader.AddToIndex(pixels[currentPixel]);
            currentPixel += 1;
        }

        return pixels;
    }

    private IChunkReader ChunkReaderSelector(byte tagByte, out int chunkLength)
    {
        if (_runReader.CanReadChunk(tagByte, out chunkLength))
            return _runReader;
        if (_indexReader.CanReadChunk(tagByte, out chunkLength))
            return _indexReader;
        if (_colorReader.CanReadChunk(tagByte, out chunkLength))
            return _colorReader;

        throw new NotImplementedException();
    }
}
