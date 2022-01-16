using System.Drawing;
using System.IO;
using QOI.Core;

namespace QOI.NET
{
    public class QoiBitmapEncoder
    {
        private readonly QoiEncoder _qoiEncoder = new();

        public byte[] Write(Bitmap image)
        {
            using var stream = new MemoryStream();
            _qoiEncoder.Write(image.ToQoiImage(), stream);
            return stream.ToArray();
        }
    }
}
