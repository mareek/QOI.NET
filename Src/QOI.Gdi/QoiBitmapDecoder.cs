using System.Drawing;
using System.IO;
using QOI.Core;

namespace QOI.Gdi;

public class QoiBitmapDecoder
{
    private readonly QoiDecoder _qoiDecoder = new();

    public Bitmap Read(string filePath)
    {
        using var stream = File.OpenRead(filePath);
        return Read(stream);
    }

    public Bitmap Read(Stream stream)
    {
        var imageWriter = new BitmapImageWriter();
        _qoiDecoder.Read(stream, imageWriter);
        return imageWriter.GetImage();
    }
}
