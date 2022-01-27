using System.IO;

namespace QOI.Core.Chunk;

internal class DiffWriter
{
    private const short MinDiff = -2;
    private const short MaxDiff = 1;

    public bool CanHandlePixel(QoiColor currentPixel, QoiColor previousPixel, out QoiColorDiff diff)
    {
        static bool IsBetween(short min, short number, short max) => min <= number && number <= max;

        diff = QoiColorDiff.FromPixels(previousPixel, currentPixel);
        return diff.Adiff == 0
               && IsBetween(MinDiff, diff.Rdiff, MaxDiff)
               && IsBetween(MinDiff, diff.Gdiff, MaxDiff)
               && IsBetween(MinDiff, diff.Bdiff, MaxDiff);
    }

    public void WriteChunk(QoiColorDiff diff, Stream stream)
    {
        // 2-bit tag b01
        // 2-bit red channel difference from the previous pixel -2..1
        // 2-bit green channel difference from the previous pixel -2..1
        // 2-bit blue channel difference from the previous pixel -2..1

        var dr = diff.Rdiff - MinDiff;
        var dg = diff.Gdiff - MinDiff;
        var db = diff.Bdiff - MinDiff;
        var chunk = (byte)(dr << 4 | dg << 2 | db);

        stream.WriteByte(Tag.DIFF.Apply(chunk));
    }
}
