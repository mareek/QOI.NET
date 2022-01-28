using System.Linq;

namespace QOI.Core.Debugging;

public class QoiFileInfo
{
    public QoiFileInfo(string name, uint width, uint height, bool hasAlpha, bool isSrgb, QoiChunkInfo[] chunks)
    {
        Name = name;
        Width = width;
        Height = height;
        HasAlpha = hasAlpha;
        IsSrgb = isSrgb;
        Chunks = chunks;
    }

    public string Name { get; }
    public uint Width { get; }
    public uint Height { get; }
    public bool HasAlpha { get; }
    public bool IsSrgb { get; }
    public QoiChunkInfo[] Chunks { get; }

    public string GetDebugString()
    {
        return $"{Name}\n"
             + $"{Width} * {Height}\n"
             + $"HasAlpha: {HasAlpha}\n"
             + $"IsSrgb: {IsSrgb}\n"
             + $"Chunks: \n"
             + string.Join("\n", Chunks.Select(c => c.GetDebugString()));
    }
}
