using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

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
            memoryStream.Seek(0, SeekOrigin.Begin);
            result.StreamSource = memoryStream;
            result.EndInit();
            return result;
        }

        public static Bitmap ToWritableBitmap(this Bitmap src) 
            => src.Clone(new Rectangle(0, 0, src.Width, src.Height), PixelFormat.Format32bppArgb);
    }
}
