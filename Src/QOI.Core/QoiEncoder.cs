using System;
using System.Collections.Generic;
using System.IO;
using QOI.Core.Chunk;

namespace QOI.Core;

public class QoiEncoder
{
    private readonly RunWriter _runWriter = new();
    private readonly IndexWriter _indexWriter = new();
    private readonly DiffWriter _diffWriter = new();
    private readonly LumaWriter _lumaWriter = new();
    private readonly RgbaWriter _rgbaWriter = new();
    private readonly RgbWriter _rgbWriter = new();

    public void Write(uint width, uint height, bool hasAlpha, bool isSrgb, IEnumerable<QoiColor> pixels, Stream stream)
    {
        HeaderHelper.WriteHeader(stream, width, height, hasAlpha, isSrgb);
        EncodePixels(pixels, stream);
        WriteFooter(stream);
    }

    private void EncodePixels(IEnumerable<QoiColor> pixels, Stream stream)
    {
        // The decoder and encoder start with { r: 0, g: 0, b: 0, a: 255} as the previous pixel value
        var previousPixel = QoiColor.FromArgb(255, 0, 0, 0);

        var pixelEnnumerator = pixels.GetEnumerator();
        while (pixelEnnumerator.MoveNext())
        {
            var currentPixel = pixelEnnumerator.Current;
            while (_runWriter.CanHandlePixel(currentPixel, previousPixel))
            {
                _runWriter.WriteChunk(pixelEnnumerator, previousPixel, stream, out bool endOfFile);
                if (endOfFile)
                {
                    return;
                }
                currentPixel = pixelEnnumerator.Current;
            }

            if (_indexWriter.CanHandlePixel(currentPixel))
            {
                _indexWriter.WriteChunk(currentPixel, stream);
            }
            else if (_diffWriter.CanHandlePixel(currentPixel, previousPixel, out var diff))
            {
                _diffWriter.WriteChunk(diff, stream);
            }
            else if (_lumaWriter.CanHandlePixel(diff))
            {
                _lumaWriter.WriteChunk(diff, stream);
            }
            else if (_rgbWriter.CanHandlePixel(currentPixel, previousPixel))
            {
                _rgbWriter.WriteChunk(currentPixel, stream);
            }
            else
            {
                _rgbaWriter.WriteChunk(currentPixel, stream);
            }

            _indexWriter.AddToIndex(currentPixel);
            previousPixel = currentPixel;
        }

    }

    private static void WriteFooter(Stream stream)
    {
        // The byte stream's end is marked with 7 0x00 bytes followed by a single 0x01 byte

        Span<byte> footer = stackalloc byte[8];
        for (int i = 0; i < 7; i++)
        {
            footer[i] = 0;
        }
        footer[7] = 1;
        stream.Write(footer);
    }
}
