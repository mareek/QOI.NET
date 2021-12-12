using System;

namespace QOI.Core.Chunk;

internal class DiffReader : IChunkReader
{
    private const byte Diff8Tag = 0b10;
    private const byte Diff16Tag = 0b110;
    private const byte Diff24Tag = 0b1110;

    public bool CanReadChunk(byte tagByte, out int chunkLength)
    {
        if ((tagByte >> 6) == Diff8Tag)
        {
            chunkLength = 1;
            return true;
        }
        else if ((tagByte >> 5) == Diff16Tag)
        {
            chunkLength = 2;
            return false;
            return true;
        }
        else if ((tagByte >> 4) == Diff24Tag)
        {
            chunkLength = 3;
            return false;
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
        => chunk.Length switch
        {
            1 => ParseDiff8(chunk[0]),
            _ => throw new NotImplementedException()
        };

    private QoiColorDiff ParseDiff8(byte chunk)
    {
        int rDiff = TruncateToBits((chunk >> 4), 2) - 1;
        int gDiff = TruncateToBits((chunk >> 2), 2) - 1;
        int bDiff = TruncateToBits(chunk, 2) - 1;
        return new(0, (short)rDiff, (short)gDiff, (short)bDiff);
    }

    private static int TruncateToBits(int number, byte bitCount)
        => number & ((1 << bitCount) - 1);
}
