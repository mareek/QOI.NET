using System;

namespace QOI.Core.Interface;

public abstract class RawPixelsImageWriter : IImageWriter
{
    protected int _width;
    protected int _height;
    protected byte[]? _rawPixels;

    private int _currentIndex;

    protected abstract int PixelSize { get; }

    public bool IsComplete => _rawPixels != null && _rawPixels.Length <= _currentIndex;

    public void Init(uint width, uint height, bool hasAlpha, bool isSrgb)
    {
        _width = (int)width;
        _height = (int)height;
        _rawPixels = new byte[_width * _height * PixelSize];
    }

    public void WritePixel(byte r, byte g, byte b, byte a)
    {
        if (_rawPixels == null) throw new ArgumentNullException("Image has not been initialized");

        WritePixelBytes(r, g, b, a, _rawPixels.AsSpan(_currentIndex, PixelSize));

        _currentIndex += PixelSize;
    }

    protected abstract void WritePixelBytes(byte r, byte g, byte b, byte a, Span<byte> buffer);
}
