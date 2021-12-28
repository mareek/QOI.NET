namespace QOI.Core;

internal struct QoiColorDiff
{
    public short Rdiff { get; }
    public short Gdiff { get; }
    public short Bdiff { get; }

    public QoiColorDiff(short rDiff, short gDiff, short bDiff)
    {
        Rdiff = rDiff;
        Gdiff = gDiff;
        Bdiff = bDiff;
    }

    public QoiColor GetPixel(QoiColor basePixel)
        => QoiColor.FromArgb(basePixel.A,
                             (byte)(basePixel.R + Rdiff),
                             (byte)(basePixel.G + Gdiff),
                             (byte)(basePixel.B + Bdiff));

    public static QoiColorDiff FromPixels(QoiColor basePixel, QoiColor diffPixel)
        => new QoiColorDiff((short)(diffPixel.R - basePixel.R),
                            (short)(diffPixel.G - basePixel.G),
                            (short)(diffPixel.B - basePixel.B));
}
