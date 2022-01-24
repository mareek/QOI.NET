﻿using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using QOI.Core;

namespace QOI.NET
{
    public class QoiBitmapEncoder
    {
        private readonly QoiEncoder _qoiEncoder = new();

        public byte[] Write(Bitmap image)
        {
            using var stream = new MemoryStream();
            var pixels = GetPixels(image).Select(p => (p.R, p.G, p.B, p.A));
            _qoiEncoder.Write((uint)image.Width, (uint)image.Height, true, true, pixels, stream);
            return stream.ToArray();
        }

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
}
