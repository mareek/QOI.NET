using System.Windows.Media;
using System.Windows.Media.Imaging;
using QOI.Core;
using QOI.Core.Interface;

namespace QOI.Wpf;

internal class BitmapSourceImageWriter : IImageWriter
{
    private const int PixelSize = 4;

    protected int _width;
    protected int _height;
    protected byte[]? _rawPixels;

    private int _currentIndex;

    public bool IsComplete => _rawPixels != null && _rawPixels.Length <= _currentIndex;

    public void Init(uint width, uint height, bool hasAlpha, bool isSrgb)
    {
        _width = (int)width;
        _height = (int)height;
        _rawPixels = new byte[_width * _height * PixelSize];
    }

    public void WritePixel(QoiColor color)
    {
        if (_rawPixels == null) throw new InvalidOperationException("Image has not been initialized");

        var pixelBytes = _rawPixels.AsSpan(_currentIndex, PixelSize);
        pixelBytes[0] = color.B;
        pixelBytes[1] = color.G;
        pixelBytes[2] = color.R;
        pixelBytes[3] = color.A;

        _currentIndex += PixelSize;
    }

    public BitmapSource GetImage()
    {
        if (_rawPixels == null) throw new InvalidOperationException("Image has not been initialized");

        var bitmapSource = BitmapSource.Create(_width,
                                               _height,
                                               96,
                                               96,
                                               PixelFormats.Bgra32,
                                               null,
                                               _rawPixels,
                                               _width * PixelSize);
        bitmapSource.Freeze();
        return bitmapSource;
    }
}
