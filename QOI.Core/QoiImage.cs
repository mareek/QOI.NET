using System;

namespace QOI.Core;

public class QoiImage
{
    private readonly QoiColor[] _pixels;

    public QoiImage(uint width, uint height, bool hasAlpha, byte colorSpace, QoiColor[] pixels)
    {
        _pixels = pixels;
        Width = width;
        Height = height;
        HasAlpha = hasAlpha;
        ColorSpace = colorSpace;
    }

    public QoiImage(uint width, uint height, QoiColor[] pixels)
        : this(width, height, true, 0b1111, pixels)
    {
    }


    public uint Width { get; }
    public uint Height { get; }

    public bool HasAlpha { get; }
    public byte ColorSpace { get; }

    public ReadOnlySpan<QoiColor> Pixels => _pixels;

    public byte[] GetBGRAPixels()
    {
        const int pixelSize = 4;
        var result = new byte[_pixels.Length * pixelSize];
        for (int i = 0; i < _pixels.Length; i++)
        {
            var pixel = _pixels[i];
            int offset = i * pixelSize;
            var buffer = result.AsSpan(offset, pixelSize);
            buffer[0] = pixel.B;
            buffer[1] = pixel.G;
            buffer[2] = pixel.R;
            buffer[3] = pixel.A;

        }
        return result;
    }
}
