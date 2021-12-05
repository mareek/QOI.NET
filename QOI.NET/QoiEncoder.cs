using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace QOI.NET
{
    public class QoiEncoder
    {
        private readonly ColorChunkWriter _colorChunkWriter = new();

        public byte[] Write(Bitmap image)
        {
            using var stream = new MemoryStream();
            stream.Write(FromatConstants.MagicBytes);

            Span<byte> uintBuffer = stackalloc byte[4];
            BinaryPrimitives.WriteUInt32BigEndian(uintBuffer, (uint)image.Width);
            stream.Write(uintBuffer);
            BinaryPrimitives.WriteUInt32BigEndian(uintBuffer, (uint)image.Height);
            stream.Write(uintBuffer);

            /*
 struct qoi_header_t {
    char [4];       // magic bytes "qoif"
    u32 width;      // image width in pixels (BE)
    u32 height;     // image height in pixels (BE)
     u8 channels;   // must be 3 (RGB) or 4 (RGBA)
     u8 colorspace; // a bitmap 0000rgba where 
                    //   - a zero bit indicates sRGBA, 
                    //   - a one bit indicates linear (user interpreted)
                    //   colorspace for each channel
};
 */

            stream.WriteByte(4); //Channels
            stream.WriteByte(0b0000_1111); //color space

            EncodePixels(image.GetPixels(), stream);

            return stream.ToArray();
        }

        private void EncodePixels(IEnumerable<Color> pixels, Stream stream)
        {
            foreach (var pixel in pixels)
            {
                _colorChunkWriter.Write(pixel, stream);
            }
        }
    }
}
