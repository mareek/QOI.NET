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

            WriteHeader(image, stream);
            EncodePixels(image.GetPixels(), stream);

            return stream.ToArray();
        }

        private static void WriteHeader(Bitmap image, MemoryStream stream)
        {
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

            stream.Write(FromatConstants.MagicBytes);
            
            Span<byte> widthBuffer = stackalloc byte[4];
            BinaryPrimitives.WriteUInt32BigEndian(widthBuffer, (uint)image.Width);
            stream.Write(widthBuffer);

            Span<byte> heightBuffer = stackalloc byte[4];
            BinaryPrimitives.WriteUInt32BigEndian(heightBuffer, (uint)image.Height);
            stream.Write(heightBuffer);

            stream.WriteByte(4); //Channels
            stream.WriteByte(0b0000_0000); //color space
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
