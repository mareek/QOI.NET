using System;
using System.IO;

namespace QOI.Core.Chunk;

internal class DiffWriter
{
    private const short MinDiff24 = -15;
    private const short MaxDiff24 = 16;
    private const short MinDiff16 = -7;
    private const short MaxDiff16 = 8;
    private const short MinDiff8 = -1;
    private const short MaxDiff8 = 2;

    private static bool IsBetween(short min, short number, short max) => min <= number && number <= max;

    public bool CanHandlePixel(QoiColor currentPixel, QoiColor previousPixel, out QoiColorDiff diff)
    {
        //static bool IsEncodableDiff(short componentDiff) => IsBetween(MinDiff24, componentDiff, MaxDiff24);
        static bool IsEncodableDiff(short componentDiff) => IsBetween(MinDiff16, componentDiff, MaxDiff16);

        diff = QoiColorDiff.FromPixels(previousPixel, currentPixel);
        return true //IsEncodableDiff(diff.Adiff)
               && IsBetween(MinDiff24, diff.Rdiff, MaxDiff24)
               && IsEncodableDiff(diff.Gdiff)
               && IsEncodableDiff(diff.Bdiff);
    }

    public void WriteChunk(QoiColorDiff diff, Stream stream)
    {
        if (diff.Adiff == 0
            && IsBetween(MinDiff8, diff.Rdiff, MaxDiff8)
            && IsBetween(MinDiff8, diff.Gdiff, MaxDiff8)
            && IsBetween(MinDiff8, diff.Bdiff, MaxDiff8))
        {
            WriteDiff8(diff, stream);
        }
        else if (diff.Adiff == 0
            && IsBetween(MinDiff24, diff.Rdiff, MaxDiff24)
            && IsBetween(MinDiff16, diff.Gdiff, MaxDiff16)
            && IsBetween(MinDiff16, diff.Bdiff, MaxDiff16))
        {
            WriteDiff16(diff, stream);
        }
        else
        {
            WriteDiff24(diff, stream);
        }
    }

    private void WriteDiff8(QoiColorDiff diff, Stream stream)
    {
        //QOI_DIFF_8 {
        //    u8 tag  :  2;   // b10
        //    u8 dr   :  2;   // 2-bit   red channel difference: -1..2
        //    u8 dg   :  2;   // 2-bit green channel difference: -1..2
        //    u8 db   :  2;   // 2-bit  blue channel difference: -1..2
        //}
        var dr = diff.Rdiff - MinDiff8;
        var dg = diff.Gdiff - MinDiff8;
        var db = diff.Bdiff - MinDiff8;
        var chunk = (byte)(dr << 4 | dg << 2 | db);

        stream.WriteByte(Tag.Diff8.WithTag(chunk));
    }

    private void WriteDiff16(QoiColorDiff diff, Stream stream)
    {
        //QOI_DIFF_16 {
        //    u8 tag  :  3;   // b110
        //    u8 dr   :  5;   // 5-bit   red channel difference: -15..16
        //    u8 dg   :  4;   // 4-bit green channel difference:  -7.. 8
        //    u8 db   :  4;   // 4-bit  blue channel difference:  -7.. 8
        //}
        var dr = diff.Rdiff - MinDiff24;
        var dg = diff.Gdiff - MinDiff16;
        var db = diff.Bdiff - MinDiff16;

        Span<byte> chunk = stackalloc byte[2];
        chunk[0] = (byte)dr;
        chunk[1] = (byte)(dg << 4 | db);
        Tag.Diff16.WriteTag(chunk);
        stream.Write(chunk);
    }

    private void WriteDiff24(QoiColorDiff diff, Stream stream)
    {
        //QOI_DIFF_24 {
        //    u8 tag  :  4;   // b1110
        //    u8 dr   :  5;   // 5-bit   red channel difference: -15..16
        //    u8 dg   :  5;   // 5-bit green channel difference: -15..16
        //    u8 db   :  5;   // 5-bit  blue channel difference: -15..16
        //    u8 da   :  5;   // 5-bit alpha channel difference: -15..16
        //}
    }
}
