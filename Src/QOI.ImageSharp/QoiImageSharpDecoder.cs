using System.IO;
using QOI.Core;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace QOI.ImageSharp;

public class QoiImageSharpDecoder
{
    private readonly QoiDecoder _qoiDecoder = new();

    public Image<Rgba32> Read(string filePath)
    {
        using var stream = File.OpenRead(filePath);
        return Read(stream);
    }

    public Image<Rgba32> Read(Stream stream)
    {
        var imageWriter = new ImageSharpWriter();
        _qoiDecoder.Read(stream, imageWriter);
        return imageWriter.GetImage();
    }
}
