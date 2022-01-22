using System;
using System.Windows.Media.Imaging;
using QOI.Core.Interface;

namespace QOI.Viewer;

internal class BitmapSourceImageWriter : RawPixelsImageWriter
{
    protected override int PixelSize => 4;

    protected override void WritePixelBytes(byte r, byte g, byte b, byte a, Span<byte> buffer)
    {
        buffer[0] = b;
        buffer[1] = g;
        buffer[2] = r;
        buffer[3] = a;
    }

    public BitmapSource GetImage()
    {
        if (_rawPixels == null) throw new ArgumentNullException("Image has not been initialized");

        var bitmapSource = BitmapSource.Create(_width,
                                               _height,
                                               96,
                                               96,
                                               System.Windows.Media.PixelFormats.Bgra32,
                                               null,
                                               _rawPixels,
                                               _width * PixelSize);
        bitmapSource.Freeze();
        return bitmapSource;
    }
}
