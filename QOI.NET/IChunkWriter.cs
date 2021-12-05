using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace QOI.NET
{
    internal interface IChunkWriter
    {
        bool CanHandlePixel(Color[] pixels, int currentPixel);
        void WriteChunk(Color[] pixels, ref int currentPixel, Stream stream);
    }
}
