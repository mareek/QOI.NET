using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using QOI.Core;
using static System.Drawing.Imaging.PixelFormat;

namespace QOI.NET;

public class QoiBitmapEncoder
{
    private static readonly PixelFormat[] _pixelFormatsWithAlpha = { Canonical, Format32bppArgb, Format64bppArgb, Format32bppPArgb, Format64bppPArgb };

    private readonly QoiEncoder _qoiEncoder = new();

    public void Write(Bitmap image, Stream stream)
    {
        var pixels = GetPixels(image).Select(p => QoiColor.FromArgb(p.A, p.R, p.G, p.B));
        _qoiEncoder.Write((uint)image.Width, (uint)image.Height, HasAlpha(image), true, pixels, stream);
    }

    private bool HasAlpha(Bitmap image) => _pixelFormatsWithAlpha.Contains(image.PixelFormat);

    private static IEnumerable<Color> GetPixels(Bitmap image)
    {
        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                yield return image.GetPixel(x, y);
            }
        }
    }
}
