using System;

namespace QOI.Core.Chunk;

internal class DiffReader : IChunkReader
{
    private const byte Diff8Tag = 0b10;
    private const byte Diff16Tag = 0b110;
    private const byte Diff24Tag = 0b1110;

    public bool CanReadChunk(byte tagByte, out int chunkLength)
    {
        chunkLength = 0;
        return false;

        if ((tagByte >> 6) == Diff8Tag)
        {
            chunkLength = 1;
            return true;
        }
        else if ((tagByte >> 5) == Diff16Tag)
        {
            chunkLength = 2;
            return true;
        }
        else if ((tagByte >> 4) == Diff24Tag)
        {
            chunkLength = 3;
            return true;
        }
        else
        {
            chunkLength = 0;
            return false;
        }
    }

    public void WritePixels(QoiColor[] pixels, ref int currentPixel, Span<byte> chunk)
    {
        var diff = ParseDiff(chunk);
        var previousPixel = pixels[currentPixel - 1];
        pixels[currentPixel] = diff.GetPixel(previousPixel);
    }

    private QoiColorDiff ParseDiff(Span<byte> chunk)
    {
        throw new NotImplementedException();
    }
}
