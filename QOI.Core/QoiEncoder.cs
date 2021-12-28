using System;
using System.IO;
using QOI.Core.Chunk;

namespace QOI.Core;

public class QoiEncoder
{
    private readonly RunWriter _runWriter = new();
    private readonly IndexWriter _indexWriter = new();
    private readonly DiffWriter _diffWriter = new();
    private readonly RgbaWriter _rgbaWriter = new();
    private readonly RgbWriter _rgbWriter = new();

    public void Write(QoiImage image, Stream stream)
    {
        HeaderHelper.WriteHeader(stream, image.Width, image.Height, image.HasAlpha, image.ColorSpace);
        EncodePixels(image.Pixels, stream);
        WriteFooter(stream);
    }

    private void EncodePixels(ReadOnlySpan<QoiColor> pixels, Stream stream)
    {
        if (pixels.Length == 0)
            return;

        // The decoder and encoder start with { r: 0, g: 0, b: 0, a: 255} as the previous pixel value
        var previousPixel = QoiColor.FromArgb(255, 0, 0, 0);

        for (int currentPixelIndex = 0; currentPixelIndex < pixels.Length; currentPixelIndex++)
        {
            QoiColor currentPixel = pixels[currentPixelIndex];

            if (_runWriter.CanHandlePixel(currentPixel, previousPixel))
            {
                _runWriter.WriteChunk(pixels, currentPixelIndex, previousPixel, stream, out int runlength);
                currentPixelIndex += runlength - 1;
            }
            else if (_indexWriter.CanHandlePixel(currentPixel))
            {
                _indexWriter.WriteChunk(currentPixel, stream);
            }
            else if (_diffWriter.CanHandlePixel(currentPixel, previousPixel, out var diff))
            {
                _diffWriter.WriteChunk(diff, stream);
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
