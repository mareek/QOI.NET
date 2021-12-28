using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using QOI.Core;

namespace QOI.NET
{
    internal static class BitmapExtension
    {
        public static IEnumerable<Color> GetPixels(this Bitmap image)
        {
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    yield return image.GetPixel(x, y);
                }
            }
        }

        public static QoiColor ToQoiColor(this Color color)
            => QoiColor.FromArgb(color.A, color.R, color.G, color.B);

        public static Color ToColor(this QoiColor qoiColor)
            => Color.FromArgb(qoiColor.A, qoiColor.R, qoiColor.G, qoiColor.B);

        public static QoiImage ToQoiImage(this Bitmap image)
        {
            QoiColor[] pixels = image.GetPixels().Select(c => c.ToQoiColor()).ToArray();
            return new QoiImage((uint)image.Width, (uint)image.Height, pixels);
        }
        public static Bitmap ToBitmap(this QoiImage qoiImage)
        {
            var image = new Bitmap((int)qoiImage.Width, (int)qoiImage.Height);
            for (int y = 0; y < image.Height; y++)
            {
                int offset = y * image.Width;
                for (int x = 0; x < image.Width; x++)
                {
                    image.SetPixel(x, y, qoiImage.Pixels[x + offset].ToColor());
                }
            }

            return image;
        }
    }
}
