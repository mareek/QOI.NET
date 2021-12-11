using System;
using System.IO;
using QOI.Core.Chunk;

namespace QOI.Core;

public class QoiEncoder
{
    private readonly RunWriter _run8Writer = new();
    private readonly IndexWriter _indexWriter = new();
    private readonly DiffWriter _diffWriter = new();
    private readonly ColorWriter _colorWriter = new();

    public void Write(QoiImage image, Stream stream)
    {
        HeaderHelper.WriteHeader(stream, image.Width, image.Height, image.HasAlpha, image.ColorSpace);
        EncodePixels(image.Pixels, stream);
    }

    private void EncodePixels(ReadOnlySpan<QoiColor> pixels, Stream stream)
    {
        if (pixels.Length == 0)
            return;

        //First pixel is always a color chunk
        QoiColor currentPixel = pixels[0];
        _colorWriter.WriteChunk(currentPixel, stream);

        int currentPixelIndex = 1;
        while (currentPixelIndex < pixels.Length)
        {
            QoiColor previousPixel = currentPixel;
            currentPixel = pixels[currentPixelIndex];

            if (_run8Writer.CanHandlePixel(currentPixel, previousPixel))
            {
                _run8Writer.WriteChunk(pixels, currentPixelIndex, previousPixel, stream, out int runlength);
                currentPixelIndex += runlength;
            }
            else if (_indexWriter.CanHandlePixel(currentPixel))
            {
                _indexWriter.WriteChunk(currentPixel, stream);
                currentPixelIndex += 1;
            }
            else if (!_diffWriter.TryWrite(currentPixel, previousPixel, stream))
            {
                _colorWriter.WriteChunk(currentPixel, stream);
                currentPixelIndex += 1;
            }

            _indexWriter.AddToIndex(currentPixel);
        }
    }
}
