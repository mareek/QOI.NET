using System.IO;
using QOI.Core;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace QOI.ImageSharp;

public class QoiImageSharpEncoder
{
    private readonly QoiEncoder _qoiEncoder = new();

    public void Write<TPixel>(Image<TPixel> image, Stream stream)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        var hasAlpha = typeof(TPixel) == typeof(Rgba32);
        var pixels = GetPixels(image);
        _qoiEncoder.Write((uint)image.Width, (uint)image.Height, hasAlpha, true, pixels, stream);
    }

    private static QoiColor[] GetPixels<TPixel>(Image<TPixel> image)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        int i = 0;
        var result = new QoiColor[image.Width * image.Height];
        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                Rgba32 pixel = default;
                image[x, y].ToRgba32(ref pixel); 
                result[i++] = QoiColor.FromArgb(pixel.A, pixel.R, pixel.G, pixel.B);
            }
        }

        return result;
    }
}
