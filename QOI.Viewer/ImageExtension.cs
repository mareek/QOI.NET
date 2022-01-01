using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;
using QOI.Core;

namespace QOI.NET
{
    internal static class ImageExtension
    {
        public static BitmapImage ToImageSource(this Image source)
        {
            // Don't dispose this stream
            MemoryStream memoryStream = new();
            source.Save(memoryStream, ImageFormat.Png);

            BitmapImage result = new();
            result.BeginInit();
            memoryStream.Position = 0;
            result.StreamSource = memoryStream;
            result.EndInit();
            return result;
        }

        public static BitmapSource ToImageSource(this QoiImage qoiImage)
        {
            int width = (int)qoiImage.Width;
            int height = (int)qoiImage.Height;
            Array pixels = qoiImage.GetBGRAPixels();
            return BitmapSource.Create(width,
                                       height,
                                       96,
                                       96,
                                       System.Windows.Media.PixelFormats.Bgra32,
                                       null,
                                       pixels,
                                       width * 4);
        }

        public static Bitmap ToWritableBitmap(this Bitmap src)
            => src.Clone(new Rectangle(0, 0, src.Width, src.Height), PixelFormat.Format32bppArgb);
    }
}
