using System;
using System.Collections.Generic;
using System.Text;

namespace QOI.Core.Interface
{
    internal class QoiImageWriter : IImageWriter
    {
        public uint Width { get; private set; }
        public uint Height { get; private set; }
        public bool HasAlpha { get; private set; }
        public bool IsSrgb { get; private set; }
        public QoiColor[] Pixels { get; private set; }

        public void Init(uint width, uint height, bool hasAlpha, bool isSrgb)
        {
            Width = width;
            Height = height;
            HasAlpha = hasAlpha;
            IsSrgb = isSrgb;
        }

        public void WritePixel(byte r, byte g, byte b, byte a)
        {
            throw new NotImplementedException();
        }
    }
}
