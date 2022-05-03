using System.Drawing;
using QOI.Core;

namespace QOI.Bench;

internal class RawEncoder
{
    private readonly QoiEncoder _qoiEncoder = new();

    public void Encode(uint width, uint height, QoiColor[] pixels)
    {
        _qoiEncoder.Write(width, height, true, true, pixels, new ForgetStream());
    }

    public static (uint width, uint height, QoiColor[] pixels) GetImageData(Bitmap image)
    {
        var pixels = new QoiColor[image.Width * image.Height];

        for (int y = 0; y < image.Height; y++)
            for (int x = 0; x < image.Width; x++)
            {
                var color = image.GetPixel(x, y);
                pixels[y * image.Width + x] = QoiColor.FromArgb(color.A, color.R, color.G, color.B);
            }

        return ((uint)image.Width, (uint)image.Height, pixels);
    }
}
