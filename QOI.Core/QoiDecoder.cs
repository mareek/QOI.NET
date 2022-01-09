using System;
using System.IO;
using QOI.Core.Chunk;

namespace QOI.Core;

public class QoiDecoder
{
    private readonly RgbaReader _rgbaReader = new();
    private readonly RgbReader _rgbReader = new();
    private readonly RunReader _runReader = new();
    private readonly IndexReader _indexReader = new();
    private readonly DiffReader _diffReader = new();
    private readonly LumaReader _lumaReader = new();

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
        var currentPixelIndex = 0;
        while (currentPixelIndex < pixels.Length)
        {
            stream.Read(chunkBuffer[0..1]);
            var chunkReader = ChunkReaderSelector(chunkBuffer[0]);
            if (chunkReader.ChunkLength > 1)
            {
                stream.Read(chunkBuffer[1..chunkReader.ChunkLength]);
            }

            chunkReader.WritePixels(pixels, ref currentPixelIndex, chunkBuffer[0..chunkReader.ChunkLength]);

            _indexReader.AddToIndex(pixels[currentPixelIndex]);
            currentPixelIndex += 1;
        }

        return pixels;
    }

    private IChunkReader ChunkReaderSelector(byte tagByte)
    {
        if (Tag.RGBA.IsPresent(tagByte))
            return _rgbaReader;
        if (Tag.RGB.IsPresent(tagByte))
            return _rgbReader;
        if (Tag.RUN.IsPresent(tagByte))
            return _runReader;
        if (Tag.INDEX.IsPresent(tagByte))
            return _indexReader;
        if (Tag.DIFF.IsPresent(tagByte))
            return _diffReader;
        if (Tag.LUMA.IsPresent(tagByte))
            return _lumaReader;
        
        throw new FormatException($"Unknown tag : {tagByte:X4}");
    }
}
