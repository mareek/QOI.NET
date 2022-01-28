namespace QOI.Core.Interface;

public interface IImageWriter
{
    bool IsComplete { get; }
    void Init(uint width, uint height, bool hasAlpha, bool isSrgb);
    void WritePixel(QoiColor color);
}
