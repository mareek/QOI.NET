using System;
using System.IO;
using QOI.Core;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace QOI.ImageSharp;

public class QoiImageSharpDecoder
{
    private readonly QoiDecoder _qoiDecoder = new();

    public Image Read(string filePath)
    {
        using var stream = File.OpenRead(filePath);
        return Read(stream);
    }

    public Image Read(Stream stream)
    {
        var qoiImage = _qoiDecoder.Read(stream);
        if (qoiImage.HasAlpha)
            return QoiImageToImage(qoiImage, p => new Rgba32(p.R, p.G, p.B, p.A));
        else
            return QoiImageToImage(qoiImage, p => new Rgb24(p.R, p.G, p.B));

    }

    private static Image<TPixel> QoiImageToImage<TPixel>(QoiImage qoiImage, Func<QoiColor, TPixel> pixelConverter)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        var image = new Image<TPixel>((int)qoiImage.Width, (int)qoiImage.Height);

        for (int y = 0; y < qoiImage.Height; y++)
        {
            Span<TPixel> rowSpan = image.GetPixelRowSpan(y);
            for (int x = 0; x < qoiImage.Width; x++)
            {
                var pixelIndex = y * qoiImage.Width + x;
                rowSpan[x] = pixelConverter(qoiImage.Pixels[pixelIndex]);
            }
        }

        return image;
    }
}
