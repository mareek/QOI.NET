using System;

namespace QOI.Core.Chunk;

internal struct Tag
{
    public static readonly Tag Run8 = new(0b010, 3);
    public static readonly Tag Run16 = new(0b011, 3);
    public static readonly Tag Index = new(0b00, 2);
    public static readonly Tag Diff8 = new(0b10, 2);
    public static readonly Tag Diff16 = new(0b110, 3);
    public static readonly Tag Diff24 = new(0b1110, 4);
    public static readonly Tag Color = new(0b1111, 4);

    private readonly byte _tag;
    private readonly byte _tagLength;

    public Tag(byte tag, byte tagLength)
    {
        _tag = tag;
        _tagLength = tagLength;
    }

    public bool HasTag(byte tagByte)
        => _tag == (tagByte >> (8 - _tagLength));

    public byte WithTag(byte tagByte)
    {
        int eraserMask = 0b1111_1111 >> _tagLength;
        int tagMask = _tag << (8 - _tagLength);
        int cleanTagByte = tagByte & eraserMask;
        return (byte)(cleanTagByte | tagMask);
    }

    public void WriteTag(Span<byte> chunk)
    {
        chunk[0] = WithTag(chunk[0]);
    }
}
