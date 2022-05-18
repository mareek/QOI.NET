using System;
using QOI.Core;
using QOI.Core.Interface;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace QOI.ImageSharp;

internal class ImageSharpWriter : IImageWriter
{
    private Image<Rgba32>? _image;
    private int _currentIndex = 0;

    public bool IsComplete => _image != null && _currentIndex == _image.Width * _image.Height;

    public void Init(uint width, uint height, bool hasAlpha, bool isSrgb)
    {
        _image = new Image<Rgba32>((int)width, (int)height);
    }

    public void WritePixel(QoiColor color)
    {
        if (_image == null) throw new InvalidOperationException("Image has not been initialized");

        int width = _image.Width;
        int x = _currentIndex % width;
        int y = _currentIndex / width;

        _image.GetPixelRowSpan(y)[x] = new Rgba32(color.R, color.G, color.B, color.A);
        _currentIndex++;
    }

    public Image<Rgba32> GetImage()
    {
        return _image ?? throw new InvalidOperationException("Image has not been initialized");
    }

}
