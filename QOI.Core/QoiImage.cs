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
}
