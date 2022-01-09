using System;
using System.Windows.Media.Imaging;
using QOI.Core;

namespace QOI.NET
{
    internal static class ImageExtension
    {
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
    }
}
