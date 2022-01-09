using System.Buffers;

namespace QOI.Core.Interface
{
    internal class QoiImageWriter : IImageWriter
    {
        private int _currentPixelIndex = 0;
        private bool _initialized = false;

        public QoiImageWriter()
        {
            Pixels = ArrayPool<QoiColor>.Shared.Rent(0);
        }

        public uint Width { get; private set; }
        public uint Height { get; private set; }
        public bool HasAlpha { get; private set; }
        public bool IsSrgb { get; private set; }
        public QoiColor[] Pixels { get; private set; }

        public bool IsComplete => _initialized && Pixels.Length <= _currentPixelIndex;

        public void Init(uint width, uint height, bool hasAlpha, bool isSrgb)
        {
            Width = width;
            Height = height;
            HasAlpha = hasAlpha;
            IsSrgb = isSrgb;

            ArrayPool<QoiColor>.Shared.Return(Pixels);
            Pixels = new QoiColor[(height * width)];

            _currentPixelIndex = 0;
            _initialized = true;
        }

        public void WritePixel(byte r, byte g, byte b, byte a)
        {
            Pixels[_currentPixelIndex++] = QoiColor.FromArgb(a, r, g, b);
        }

        public QoiImage GetImage() => new(Width, Height, HasAlpha, IsSrgb, Pixels);
    }
}
