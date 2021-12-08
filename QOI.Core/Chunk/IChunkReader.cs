using System;

namespace QOI.Core.Chunk
{
    internal interface IChunkReader
    {
        bool CanReadChunk(byte tagByte, out int chunkLength);
        void WritePixels(QoiColor[] pixels, ref int currentPixel, Span<byte> chunk);
    }
}
