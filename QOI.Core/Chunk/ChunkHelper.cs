namespace QOI.Core.Chunk;

internal static class ChunkHelper
{
    public const short MinDiff = -2;
    public const short MaxDiff = 1;

    public static QoiColor GetPreviousPixel(QoiColor[] pixels, int currentPixel)
        => currentPixel > 0 ? pixels[currentPixel - 1] : QoiColor.FromRgb(0, 0, 0);

}
