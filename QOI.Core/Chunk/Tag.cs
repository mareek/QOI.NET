using System;

namespace QOI.Core.Chunk;

internal static class Tag
{
    public const byte Run8 = 0b010;
    public const byte Run16 = 0b011;

    public const byte Index = 0b00;

    public const byte Diff8 = 0b10;
    public const byte Diff16 = 0b110;
    public const byte Diff24 = 0b1110;

    public const byte Color = 0b1111;

    public static bool HasTag(this byte tagByte, byte tag, byte tagLength)
        => tag == (tagByte >> (8 - tagLength));

    public static byte WithTag(this byte tagByte, byte tag, byte tagLength)
    {
        int eraserMask = 0b1111_1111 >> tagLength;
        int tagMask = tag << (8 - tagLength);
        int cleanTagByte = tagByte & eraserMask;
        return (byte)(cleanTagByte | tagMask);
    }

    public static void WriteTag(this Span<byte> chunk, byte tag, byte tagLength)
    {
        chunk[0] = chunk[0].WithTag(tag, tagLength);
    }
}
