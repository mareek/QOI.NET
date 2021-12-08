using System;
using System.Buffers.Binary;
using System.IO;

namespace QOI.Core;

internal static class HeaderHelper
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

    public static readonly byte[] MagicBytes = { 0x71, 0x6F, 0x69, 0x66 };
    public const int HeaderLength = 14;

    public static void WriteHeader(Stream stream, uint width, uint height, bool hasAlpha, byte colorspace)
    {
        Span<byte> header = stackalloc byte[HeaderLength];
        MagicBytes.CopyTo(header[0..4]);
        BinaryPrimitives.WriteUInt32BigEndian(header[4..8], width);
        BinaryPrimitives.WriteUInt32BigEndian(header[8..12], height);
        header[12] = (byte)(hasAlpha ? 4 : 3);
        header[13] = (byte)(0b0000_1111 & colorspace);
        stream.Write(header);
    }

    public static (uint width, uint height, bool hasAlpha, byte colorspace) ReadHeader(Stream stream)
    {
        Span<byte> header = stackalloc byte[HeaderLength];
        stream.Read(header);

        if (!header[0..4].SequenceEqual(MagicBytes))
            throw new FormatException("This is not a valid QOI image");

        var width = BinaryPrimitives.ReadUInt32BigEndian(header[4..8]);
        var height = BinaryPrimitives.ReadUInt32BigEndian(header[8..12]);
        var hasAlpha = header[12] == 4;
        byte colorspace = header[13];

        return (width, height, hasAlpha, colorspace);
    }
}
