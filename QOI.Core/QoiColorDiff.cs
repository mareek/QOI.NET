namespace QOI.Core;

internal struct QoiColorDiff
{
    public short Rdiff { get; }
    public short Gdiff { get; }
    public short Bdiff { get; }
    public short Adiff { get; }

    public QoiColorDiff(short aDiff, short rDiff, short gDiff, short bDiff)
    {
        Adiff = aDiff;
        Rdiff = rDiff;
        Gdiff = gDiff;
        Bdiff = bDiff;
    }

    public QoiColor GetPixel(QoiColor basePixel)
        => QoiColor.FromArgb((byte)(basePixel.A + Adiff),
                             (byte)(basePixel.R + Rdiff),
                             (byte)(basePixel.G + Gdiff),
                             (byte)(basePixel.B + Bdiff));

    public static QoiColorDiff FromPixels(QoiColor basePixel, QoiColor diffPixel)
        => new QoiColorDiff((short)(diffPixel.A - basePixel.A),
                            (short)(diffPixel.R - basePixel.R),
                            (short)(diffPixel.G - basePixel.G),
                            (short)(diffPixel.B - basePixel.B));
}
