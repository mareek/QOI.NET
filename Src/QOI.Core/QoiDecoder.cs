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

    public static bool IsQoiImage(Stream stream)
    {
        Span<byte> firstBytes = stackalloc byte[HeaderHelper.MagicBytes.Length];

        stream.Read(firstBytes);
        return firstBytes.SequenceEqual(HeaderHelper.MagicBytes);
    }

    public QoiImage Read(Stream stream)
    {
        var (width, height, hasAlpha, isSrgb) = HeaderHelper.ReadHeader(stream);

        var pixels = DecodePixels(stream, width * height);

        return new QoiImage(width, height, hasAlpha, isSrgb, pixels);
    }

    private QoiColor[] DecodePixels(Stream stream, uint pixelCount)
    {
        Span<byte> chunkBuffer = stackalloc byte[5];

        var result = new QoiColor[pixelCount];
        var previousPixel = QoiColor.FromArgb(255, 0, 0, 0);

        int pixelIndex = 0;
        while (pixelIndex < pixelCount)
        {
            stream.Read(chunkBuffer[0..1]);
            var chunkReader = ChunkReaderSelector(chunkBuffer[0]);
            if (chunkReader.ChunkLength > 1)
            {
                stream.Read(chunkBuffer[1..chunkReader.ChunkLength]);
            }
            Span<byte> chunk = chunkBuffer[0..chunkReader.ChunkLength];

            if (chunkReader is ISinglePixelChunkReader singlePixelReader)
            {
                previousPixel = singlePixelReader.ReadPixel(chunk, previousPixel);
                result[pixelIndex++] = previousPixel;
                _indexReader.AddToIndex(previousPixel);
            }
            else if (chunkReader is RunReader runReader)
            {
                int runLength = runReader.GetRunLength(chunk);
                result.AsSpan(pixelIndex, runLength)
                      .Fill(previousPixel);
                pixelIndex += runLength;
            }
            else 
                throw new NotSupportedException($"Unknown chunk reader: {chunkReader.GetType().Name}");

        }

        return result;
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