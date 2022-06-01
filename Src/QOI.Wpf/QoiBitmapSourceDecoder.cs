using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using QOI.Core;

namespace QOI.Wpf;

public class QoiBitmapSourceDecoder
{
    private readonly QoiDecoder _qoiDecoder = new();

    public BitmapSource Read(string filePath)
    {
        using var stream = File.OpenRead(filePath);
        return Read(stream);
    }

    public BitmapSource Read(Stream stream)
    {
        var qoiImage = _qoiDecoder.Read(stream);
        return QoiImageToBitmapSource(qoiImage);
    }

    private static BitmapSource QoiImageToBitmapSource(QoiImage qoiImage)
    {
        int height = (int)qoiImage.Height;
        int width = (int)qoiImage.Width;
        int pixelByteSize = qoiImage.HasAlpha ? 4 : 3;

        byte[] rawPixels = new byte[width * height * pixelByteSize];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int pixelIndex = y * width + x;
                var pixel = qoiImage.Pixels[pixelIndex];
                var pixelBytes = rawPixels.AsSpan(pixelIndex * pixelByteSize, pixelByteSize);

                if (qoiImage.HasAlpha)
                {
                    pixelBytes[0] = pixel.B;
                    pixelBytes[1] = pixel.G;
                    pixelBytes[2] = pixel.R;
                    pixelBytes[3] = pixel.A;
                }
                else
                {
                    pixelBytes[0] = pixel.R;
                    pixelBytes[1] = pixel.G;
                    pixelBytes[2] = pixel.B;
                }
            }
        }

        PixelFormat pixelFormat = qoiImage.HasAlpha ? PixelFormats.Bgra32 : PixelFormats.Rgb24;
        var bitmapSource = BitmapSource.Create(width,
                                               height,
                                               96,
                                               96,
                                               pixelFormat,
                                               null,
                                               rawPixels,
                                               width * pixelByteSize);

        bitmapSource.Freeze();
        return bitmapSource;
    }
}
