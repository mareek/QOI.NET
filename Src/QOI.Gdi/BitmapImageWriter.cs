using System;
using System.Drawing;
using System.Drawing.Imaging;
using QOI.Core;
using QOI.Core.Interface;

namespace QOI.Gdi;

internal class BitmapImageWriter : IImageWriter
{
    private Bitmap? _bitmap;
    private int _currentIndex = 0;

    public bool IsComplete => _bitmap != null && (_bitmap.Width * _bitmap.Height) <= _currentIndex;

    public void Init(uint width, uint height, bool hasAlpha, bool isSrgb)
    {
        _bitmap = new Bitmap((int)width, (int)height, PixelFormat.Format32bppArgb);
    }

    public void WritePixel(QoiColor color)
    {
        if (_bitmap == null) throw new InvalidOperationException("Image has not been initialized");

        int width = _bitmap.Width;
        var x = _currentIndex % width;
        var y = _currentIndex / width;
        _bitmap.SetPixel(x, y, Color.FromArgb(color.A, color.R, color.G, color.B));

        _currentIndex += 1;
    }

    public Bitmap GetImage()
    {
        if (_bitmap == null) throw new InvalidOperationException("Image has not been initialized");
        return _bitmap;
    }
}
