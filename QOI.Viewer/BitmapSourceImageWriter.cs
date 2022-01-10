using System;
using System.Windows.Media.Imaging;
using QOI.Core.Interface;

namespace QOI.Viewer;

internal class BitmapSourceImageWriter : IImageWriter
{
    private int _width;
    private int _height;
    private int _pixelSize = 4;
    private byte[]? _rawPixels;
    private int _currentIndex;
    public bool IsComplete => _rawPixels != null && _rawPixels.Length <= _currentIndex;

    public void Init(uint width, uint height, bool hasAlpha, bool isSrgb)
    {
        _width = (int)width;
        _height = (int)height;
        _rawPixels = new byte[_width * _height * _pixelSize];
    }

    public void WritePixel(byte r, byte g, byte b, byte a)
    {
        if (_rawPixels == null) throw new ArgumentNullException("Image has not been initialized");

        _rawPixels[_currentIndex] = b;
        _rawPixels[_currentIndex + 1] = g;
        _rawPixels[_currentIndex + 2] = r;
        _rawPixels[_currentIndex + 3] = a;

        _currentIndex += _pixelSize;
    }

    public BitmapSource GetImage()
    {
        if (_rawPixels == null) throw new ArgumentNullException("Image has not been initialized");

        return BitmapSource.Create(_width,
                                   _height,
                                   96,
                                   96,
                                   System.Windows.Media.PixelFormats.Bgra32,
                                   null,
                                   _rawPixels,
                                   _width * _pixelSize);
    }
}
