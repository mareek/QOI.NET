using System.IO;
using System.Windows.Media.Imaging;
using QOI.Core;

namespace QOI.Wpf;

public class QoiBitmapSourceDecoder
{
    private readonly QoiDecoder _qoiDecoder = new();

    public BitmapSource Read(string filePath)
    {
        using var stream = File.OpenRead(filePath);
        return Read(stream);
    }

    public BitmapSource Read(Stream stream)
    {
        BitmapSourceImageWriter imageWriter = new ();
        _qoiDecoder.Read(stream, imageWriter);
        return imageWriter.GetImage();
    }
}
