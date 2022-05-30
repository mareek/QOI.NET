namespace QOI.Core;

public class QoiImage
{
    public QoiImage(uint width, uint height, bool hasAlpha, bool isSrgb, QoiColor[] pixels)
    {
        Width = width;
        Height = height;
        HasAlpha = hasAlpha;
        IsSrgb = isSrgb;
        Pixels = pixels;
    }

    public uint Width { get; }
    public uint Height { get; }
    public bool HasAlpha { get; }
    public bool IsSrgb { get; }
    public QoiColor[] Pixels { get; }
}
