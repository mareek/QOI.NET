using System.IO;

namespace QOI.Core.Chunk;

internal class DiffWriter
{
    public bool CanHandlePixel(QoiColor currentPixel, QoiColor previousPixel, out QoiColorDiff diff)
    {
        static bool IsBetween(short min, short number, short max) => min <= number && number <= max;
        static bool IsEncodableDiff(short componentDiff) => IsBetween(ChunkHelper.MinDiff, componentDiff, ChunkHelper.MaxDiff);

        diff = QoiColorDiff.FromPixels(previousPixel, currentPixel);
        return currentPixel.A == previousPixel.A
               && IsEncodableDiff(diff.Gdiff)
               && IsEncodableDiff(diff.Gdiff)
               && IsEncodableDiff(diff.Bdiff);
    }

    public void WriteChunk(QoiColorDiff diff, Stream stream)
    {
        // 2-bit tag b01
        // 2-bit red channel difference from the previous pixel -2..1
        // 2-bit green channel difference from the previous pixel -2..1
        // 2-bit blue channel difference from the previous pixel -2..1

        var dr = diff.Rdiff - ChunkHelper.MinDiff;
        var dg = diff.Gdiff - ChunkHelper.MinDiff;
        var db = diff.Bdiff - ChunkHelper.MinDiff;
        var chunk = (byte)(dr << 4 | dg << 2 | db);

        stream.WriteByte(Tag.DIFF.Apply(chunk));
    }
}
