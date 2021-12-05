using System.Drawing;
using System.IO;

namespace QOI.NET.Chunk
{
    internal interface IChunkWriter
    {
        bool CanHandlePixel(Color[] pixels, int currentPixel);
        void WriteChunk(Color[] pixels, ref int currentPixel, Stream stream);
    }
}
