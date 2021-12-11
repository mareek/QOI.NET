﻿using System;

namespace QOI.Core.Chunk;

internal class IndexReader : IChunkReader
{
    private readonly PixelIndex _pixelIndex = new();

    public void AddToIndex(QoiColor pixel) => _pixelIndex.Add(pixel);

    public bool CanReadChunk(byte tagByte, out int chunkLength)
    {
        chunkLength = 1;
        return (tagByte >> 6) == 0b00;
    }

    public void WritePixels(QoiColor[] pixels, ref int currentPixel, Span<byte> chunk)
    {
        pixels[currentPixel] = _pixelIndex.Get(chunk[0]);
    }
}