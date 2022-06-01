using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using QOI.Core;

namespace QOI.Gdi;

public class QoiBitmapDecoder
{
    private readonly QoiDecoder _qoiDecoder = new();

    public Bitmap Read(string filePath)
    {
        using var stream = File.OpenRead(filePath);
        return Read(stream);
    }

    public Bitmap Read(Stream stream)
    {
        var qoiImage = _qoiDecoder.Read(stream);
        return QoiImageToBitmap(qoiImage);
    }

    private static Bitmap QoiImageToBitmap(QoiImage qoiImage)
    {
        var pixelFormat = qoiImage.HasAlpha ? PixelFormat.Format32bppArgb : PixelFormat.Format24bppRgb;
        Bitmap bitmap = new((int)qoiImage.Width, (int)qoiImage.Height, pixelFormat);

        for (int y = 0; y < qoiImage.Height; y++)
        {
            for (int x = 0; x < qoiImage.Width; x++)
            {
                var pixelIndex = y * qoiImage.Width + x;
                var pixel = qoiImage.Pixels[pixelIndex];
                int alpha = qoiImage.HasAlpha ? pixel.A : 255;
                bitmap.SetPixel(x, y, Color.FromArgb(alpha, pixel.R, pixel.G, pixel.B));
            }
        }

        return bitmap;
    }
}
