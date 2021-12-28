using System;
using System.Drawing;
using System.IO;
using QOI.Core;

namespace QOI.NET;

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
        var qoiImage = _qoiDecoder.Read(stream);
        return qoiImage.ToBitmap();
    }
}
