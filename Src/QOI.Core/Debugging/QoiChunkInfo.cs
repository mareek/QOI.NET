namespace QOI.Core.Debugging;

public struct QoiChunkInfo
{
    public QoiChunkInfo(ChunkType type, int length, int pixelCount)
    {
        Type = type;
        Length = length;
        PixelCount = pixelCount;
    }

    public ChunkType Type { get; }
    public int Length { get; }
    public int PixelCount { get; }

    public string GetDebugString() => $"{Type} \t{Length} \t{PixelCount}";
}
