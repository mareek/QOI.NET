using System;
using System.Buffers.Binary;
using System.Drawing;
using System.IO;
using System.Linq;

namespace QOI.NET
{
    public class QoiEncoder
    {
        private readonly IChunkWriter[] _chunkWriters = { new Run8ChunkWriter(), new ColorChunkWriter() };

        public byte[] Write(Bitmap image)
        {
            using var stream = new MemoryStream();

            WriteHeader(image, stream);
            EncodePixels(image.GetPixels().ToArray(), stream);

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

            stream.Write(FormatConstants.MagicBytes);

            Span<byte> widthBuffer = stackalloc byte[4];
            BinaryPrimitives.WriteUInt32BigEndian(widthBuffer, (uint)image.Width);
            stream.Write(widthBuffer);

            Span<byte> heightBuffer = stackalloc byte[4];
            BinaryPrimitives.WriteUInt32BigEndian(heightBuffer, (uint)image.Height);
            stream.Write(heightBuffer);

            stream.WriteByte(4); //Channels
            stream.WriteByte(0b0000_0000); //color space
        }

        private void EncodePixels(Color[] pixels, Stream stream)
        {
            var currentPixel = 0;
            while (currentPixel < pixels.Length)
            {
                var chunkWriter = ChunkWriterSelector(pixels, currentPixel);
                chunkWriter.WriteChunk(pixels, ref currentPixel, stream);
            }
        }

        private IChunkWriter ChunkWriterSelector(Color[] pixels, int currentPixel)
        {
            foreach (var chunkWriter in _chunkWriters)
            {
                if (chunkWriter.CanHandlePixel(pixels, currentPixel))
                {
                    return chunkWriter;
                }
            }

            throw new NotImplementedException();
        }
    }
}
