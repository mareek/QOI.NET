using System;
using System.Buffers.Binary;
using System.IO;

namespace QOI.Core;

internal static class HeaderHelper
{
    /*
    qoi_header {
        char     magic[4];   // magic bytes "qoif"
        uint32_t width;      // image width in pixels (BE)
        uint32_t height;     // image height in pixels (BE)
        uint8_t  channels;   // 3 = RGB, 4 = RGBA
        uint8_t  colorspace; // 0 = sRGB with linear alpha
                             // 1 = all channels linear
    };
    */

    public static readonly byte[] MagicBytes = { 0x71, 0x6F, 0x69, 0x66 };
    public const int HeaderLength = 14;

    public static void WriteHeader(Stream stream, uint width, uint height, bool hasAlpha, bool isSrgb)
    {
        Span<byte> header = stackalloc byte[HeaderLength];
        MagicBytes.CopyTo(header[0..4]);
        BinaryPrimitives.WriteUInt32BigEndian(header[4..8], width);
        BinaryPrimitives.WriteUInt32BigEndian(header[8..12], height);
        header[12] = (byte)(hasAlpha ? 4 : 3);
        header[13] = (byte)(isSrgb ? 0 : 1);
        stream.Write(header);
    }

    public static (uint width, uint height, bool hasAlpha, bool isSrgb) ReadHeader(Stream stream)
    {
        Span<byte> header = stackalloc byte[HeaderLength];
        stream.Read(header);

        if (!header[0..4].SequenceEqual(MagicBytes))
            throw new NotSupportedException("This is not a valid QOI image");

        var width = BinaryPrimitives.ReadUInt32BigEndian(header[4..8]);
        var height = BinaryPrimitives.ReadUInt32BigEndian(header[8..12]);
        var hasAlpha = header[12] == 4;
        var isSrgb = header[13] == 0;

        return (width, height, hasAlpha, isSrgb);
    }
}
