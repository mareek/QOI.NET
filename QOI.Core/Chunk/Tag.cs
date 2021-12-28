using System;

namespace QOI.Core.Chunk;

internal struct Tag
{
    public static readonly Tag INDEX = new(0b00, 2);
    public static readonly Tag DIFF = new(0b01, 2);
    public static readonly Tag LUMA = new(0b10, 2);
    public static readonly Tag RUN = new(0b11, 2);
    public static readonly Tag RGB = new(0b1111_1110, 8);
    public static readonly Tag RGBA = new(0b1111_1111, 8);

    private readonly byte _tag;
    private readonly byte _tagLength;

    public Tag(byte tag, byte tagLength)
    {
        _tag = tag;
        _tagLength = tagLength;
    }

    public bool IsPresent(byte tagByte)
        => _tag == (tagByte >> (8 - _tagLength));

    public byte Erase(byte tagByte)
    {
        int eraserMask = 0b1111_1111 >> _tagLength;
        return (byte)(tagByte & eraserMask);
    }

    public byte Apply(byte tagByte)
    {
        int cleanTagByte = Erase(tagByte);
        int tagMask = _tag << (8 - _tagLength);
        return (byte)(cleanTagByte | tagMask);
    }

    public void Write(Span<byte> chunk)
    {
        chunk[0] = Apply(chunk[0]);
    }
}
