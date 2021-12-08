namespace QOI.Core;

internal class PixelIndex
{
    private const int CacheSize = 64;
    private readonly QoiColor[] _cachedPixels = new QoiColor[CacheSize];

    public int GetIndex(QoiColor pixel) => (pixel.R ^ pixel.G ^ pixel.B ^ pixel.A) % CacheSize;

    public bool Exists(QoiColor pixel) => _cachedPixels[GetIndex(pixel)] == pixel;

    public void Add(QoiColor pixel) => _cachedPixels[GetIndex(pixel)] = pixel;

    public QoiColor Get(int index) => _cachedPixels[index];
}
