using System;
using System.IO;
using QOI.Core.Chunk;
using QOI.Core.Interface;

namespace QOI.Core;

public class QoiDecoder
{
    private readonly RgbaReader _rgbaReader = new();
    private readonly RgbReader _rgbReader = new();
    private readonly RunReader _runReader = new();
    private readonly IndexReader _indexReader = new();
    private readonly DiffReader _diffReader = new();
    private readonly LumaReader _lumaReader = new();

    public void Read(Stream stream, IImageWriter imageWriter)
    {
        var (width, height, hasAlpha, isSrgb) = HeaderHelper.ReadHeader(stream);

        imageWriter.Init(width, height, hasAlpha, isSrgb);
        DecodePixels(stream, imageWriter);
    }

    private void DecodePixels(Stream stream, IImageWriter imageWriter)
    {
        Span<byte> chunkBuffer = stackalloc byte[5];
        var previousPixel = QoiColor.FromArgb(255, 0, 0, 0);
        while (!imageWriter.IsComplete)
        {
            stream.Read(chunkBuffer[0..1]);
            var chunkReader = ChunkReaderSelector(chunkBuffer[0]);
            if (chunkReader.ChunkLength > 1)
            {
                stream.Read(chunkBuffer[1..chunkReader.ChunkLength]);
            }

            previousPixel = chunkReader.WritePixels(imageWriter, chunkBuffer[0..chunkReader.ChunkLength], previousPixel);

            _indexReader.AddToIndex(previousPixel);
        }
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
