using QOI.Core;
using QOI.Core.Interface;

namespace QOI.Bench;

internal class RawDecoder
{
    private readonly QoiDecoder _qoiDecoder = new();

    public QoiColor[] Read(byte[] imageData)
    {
        using var imageStream = new MemoryStream(imageData);
        var imageWriter = new BenchImageWriter();
        _qoiDecoder.Read(imageStream, imageWriter);
        return imageWriter.GetImage();
    }

    private class BenchImageWriter : IImageWriter
    {
        private QoiColor[]? _bitmap;
        private int _currentIndex = 0;

        public bool IsComplete => _bitmap != null && _currentIndex == _bitmap.Length;

        public void Init(uint width, uint height, bool hasAlpha, bool isSrgb)
        {
            _bitmap = new QoiColor[width * height];
        }

        public void WritePixel(QoiColor color)
        {
            if (_bitmap == null) throw new ArgumentNullException("Image has not been initialized");
            _bitmap[_currentIndex++] = color;
        }

        public QoiColor[] GetImage()
        {
            if (_bitmap == null) throw new ArgumentNullException("Image has not been initialized");
            return _bitmap;
        }
    }
}
