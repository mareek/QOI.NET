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

    public string GetFullReport(bool includeName = true)
        => GetHeaderReport(includeName)
           + $"Chunks: \n"
           + string.Join("\n", Chunks.Select(c => c.GetDebugString()));
    public string GetSummary(bool includeName = true)
        => GetHeaderReport(includeName)
           + $"Chunks: \n"
           + GetChunkSummary();

    private string GetHeaderReport(bool includeName)
        => (includeName ? $"{Name}\n" : string.Empty)
           + $"{Width} * {Height}\n"
           + $"HasAlpha: {HasAlpha}\n"
           + $"IsSrgb: {IsSrgb}\n";

    private string GetChunkSummary()
    {
        var chunksByType = Chunks.GroupBy(c => c.Type)
                                 .Select(g => new
                                 {
                                     Type = g.Key,
                                     ChunkCount = g.Count(),
                                     PixelCount = g.Sum(c => c.PixelCount),
                                     TotalSize = g.Sum(c => c.Length)
                                 })
                                 .OrderByDescending(c => c.ChunkCount)
                                 .Select(c => string.Join("\t", c.Type, c.ChunkCount, c.PixelCount, c.TotalSize));

        var header = string.Join("\t", "Type", "Chunk count", "Pixel count", "Total Size");
        return string.Join("\n", chunksByType.Prepend(header));
    }
}
