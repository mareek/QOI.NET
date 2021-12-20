﻿using System;

namespace QOI.Core.Chunk;

internal class DiffReader : IChunkReader
{
    public bool CanReadChunk(byte tagByte, out int chunkLength)
    {
        chunkLength = 1;
        return Tag.DIFF.IsPresent(tagByte);
    }

    public void WritePixels(QoiColor[] pixels, ref int currentPixel, Span<byte> chunk)
    {
        var diff = ParseDiff(chunk[0]);
        var previousPixel = pixels[currentPixel - 1];
        pixels[currentPixel] = diff.GetPixel(previousPixel);
    }

    private QoiColorDiff ParseDiff(byte chunk)
    {
        // 2-bit tag b01
        // 2-bit red channel difference from the previous pixel -2..1
        // 2-bit green channel difference from the previous pixel -2..1
        // 2-bit blue channel difference from the previous pixel -2..1

        int rDiff = TruncateToBits((chunk >> 4), 2) + DiffConst.MinDiff;
        int gDiff = TruncateToBits((chunk >> 2), 2) + DiffConst.MinDiff;
        int bDiff = TruncateToBits(chunk, 2) + DiffConst.MinDiff;
        return new(0, (short)rDiff, (short)gDiff, (short)bDiff);
    }

    private static int TruncateToBits(int number, byte bitCount)
        => number & ((1 << bitCount) - 1);
}
