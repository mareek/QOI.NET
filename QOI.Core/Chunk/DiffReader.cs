using System;

namespace QOI.Core.Chunk;

internal class DiffReader : IChunkReader
{
    public bool CanReadChunk(byte tagByte, out int chunkLength)
    {
        if (Tag.Diff8.IsPresent(tagByte))
        {
            chunkLength = 1;
            return true;
        }
        else if (Tag.Diff16.IsPresent(tagByte))
        {
            chunkLength = 2;
            return true;
        }
        else if (Tag.Diff24.IsPresent(tagByte))
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
            2 => ParseDiff16(chunk),
            _ => throw new NotImplementedException()
        };

    private QoiColorDiff ParseDiff8(byte chunk)
    {
        int rDiff = TruncateToBits((chunk >> 4), 2) +DiffConst.MinDiff8;
        int gDiff = TruncateToBits((chunk >> 2), 2) + DiffConst.MinDiff8;
        int bDiff = TruncateToBits(chunk, 2) + DiffConst.MinDiff8;
        return new(0, (short)rDiff, (short)gDiff, (short)bDiff);
    }

    private QoiColorDiff ParseDiff16(Span<byte> chunk)
    {
        int rDiff = TruncateToBits(chunk[0], 5) + DiffConst.MinDiff24;
        int gDiff = TruncateToBits((chunk[1] >> 4), 4) + DiffConst.MinDiff16;
        int bDiff = TruncateToBits(chunk[1], 4) + DiffConst.MinDiff16;
        return new(0, (short)rDiff, (short)gDiff, (short)bDiff);
    }

    private static int TruncateToBits(int number, byte bitCount)
        => number & ((1 << bitCount) - 1);
}
