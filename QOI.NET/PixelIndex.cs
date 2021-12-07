using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace QOI.NET
{
    internal class PixelIndex
    {
        private const int CacheSize = 64;
        private readonly Color[] _cachedPixels = new Color[CacheSize];
        
        public int GetIndex(Color pixel) => (pixel.R ^ pixel.G ^ pixel.B ^ pixel.A) % CacheSize;

        public bool Exists(Color pixel) => _cachedPixels[GetIndex(pixel)] == pixel;

        public void Add(Color pixel) => _cachedPixels[GetIndex(pixel)] = pixel;

        public Color Get(int index) => _cachedPixels[index];
    }
}
