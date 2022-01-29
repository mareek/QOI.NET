namespace QOI.Core;

internal struct QoiColorDiff
{
    public sbyte Adiff { get; }
    public sbyte Rdiff { get; }
    public sbyte Gdiff { get; }
    public sbyte Bdiff { get; }

    public QoiColorDiff(sbyte aDiff, sbyte rDiff, sbyte gDiff, sbyte bDiff)
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
        => new((sbyte)(diffPixel.A - basePixel.A),
               (sbyte)(diffPixel.R - basePixel.R),
               (sbyte)(diffPixel.G - basePixel.G),
               (sbyte)(diffPixel.B - basePixel.B));
}
