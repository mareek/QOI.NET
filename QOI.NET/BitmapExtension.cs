using System.Collections.Generic;
using System.Drawing;

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

        public static void SetPixels(this Bitmap image, Color[] pixels)
        {
            for (int y = 0; y < image.Height; y++)
            {
                int offset = y * image.Width;
                for (int x = 0; x < image.Width; x++)
                {
                    image.SetPixel(x, y, pixels[x + offset]);
                }
            }
        }
    }
}
