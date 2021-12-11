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

    public bool TryWrite(QoiColor currentPixel, QoiColor previousPixel, Stream stream)
    {
        return false;

        static bool IsBetween(short min, short number, short max) => min <= number && number <= max;
        static bool IsEncodableDiff(short componentDiff) => IsBetween(MinDiff24, componentDiff, MaxDiff24);

        var diff = QoiColorDiff.FromPixels(previousPixel, currentPixel);

        if (!IsEncodableDiff(diff.Adiff)
            || !IsEncodableDiff(diff.Rdiff)
            || !IsEncodableDiff(diff.Gdiff)
            || !IsEncodableDiff(diff.Bdiff))
        {
            return false;
        }

        if (diff.Adiff == 0
            && IsBetween(MinDiff8, diff.Rdiff, MaxDiff8)
            && IsBetween(MinDiff8, diff.Gdiff, MaxDiff8)
            && IsBetween(MinDiff8, diff.Bdiff, MaxDiff8))
        {
            WriteDiff8(diff, stream);
        }
        else if (diff.Adiff == 0
            && IsBetween(MinDiff16, diff.Rdiff, MaxDiff16)
            && IsBetween(MinDiff16, diff.Gdiff, MaxDiff16)
            && IsBetween(MinDiff16, diff.Bdiff, MaxDiff16))
        {
            WriteDiff16(diff, stream);
        }
        else
        {
            WriteDiff24(diff, stream);
        }

        return true;
    }

    private void WriteDiff8(QoiColorDiff diff, Stream stream) { }
    private void WriteDiff16(QoiColorDiff diff, Stream stream) { }
    private void WriteDiff24(QoiColorDiff diff, Stream stream) { }
}
