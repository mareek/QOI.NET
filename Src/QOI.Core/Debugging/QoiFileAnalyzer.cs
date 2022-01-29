using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using QOI.Core.Chunk;

namespace QOI.Core.Debugging;

public class QoiFileAnalyzer
{
    public QoiFileInfo AnalyzeFile(FileInfo file)
    {
        using var fileStream = file.OpenRead();
        return AnalyzeFile(file.Name, fileStream);
    }

    public QoiFileInfo AnalyzeFile(string fileName, Stream fileStream)
    {
        var (width, height, hasAlpha, isSrgb) = HeaderHelper.ReadHeader(fileStream);
        var chunks = ReadChunks(fileStream, width * height).ToArray();
        return new QoiFileInfo(fileName, width, height, hasAlpha, isSrgb, chunks);
    }

    private IEnumerable<QoiChunkInfo> ReadChunks(Stream imageStream, uint totalPixelCount)
    {
        int currentPixelCount = 0;
        do
        {
            QoiChunkInfo chunk = ReadNextChunk(imageStream);
            currentPixelCount += chunk.PixelCount;
            yield return chunk;
        } while (currentPixelCount < totalPixelCount);
    }

    private QoiChunkInfo ReadNextChunk(Stream imageStream)
    {
        Span<byte> chunkBuffer = stackalloc byte[5];

        imageStream.Read(chunkBuffer[0..1]);
        var chunkType = GetChunkType(chunkBuffer[0]);
        
        if (chunkType == ChunkType.Run)
        {
            int runLength = chunkBuffer[0] & 0b0011_1111; // erase tag bits
            return new QoiChunkInfo(chunkType, 1, runLength + 1);
        }

        int length = GetChunkLength(chunkType);
        if (length > 1)
            imageStream.Read(chunkBuffer[1..length]);
        return new QoiChunkInfo(chunkType, length, 1);
    }

    private ChunkType GetChunkType(byte tagByte)
    {
        if (Tag.RGBA.IsPresent(tagByte))
            return ChunkType.Rgba;
        if (Tag.RGB.IsPresent(tagByte))
            return ChunkType.Rgb;
        if (Tag.RUN.IsPresent(tagByte))
            return ChunkType.Run;
        if (Tag.INDEX.IsPresent(tagByte))
            return ChunkType.Index;
        if (Tag.DIFF.IsPresent(tagByte))
            return ChunkType.Diff;
        if (Tag.LUMA.IsPresent(tagByte))
            return ChunkType.Luma;

        throw new FormatException($"Unknown tag : {tagByte:X4}");
    }

    private int GetChunkLength(ChunkType chunkType)
        => chunkType switch
        {
            ChunkType.Index or ChunkType.Diff or ChunkType.Run => 1,
            ChunkType.Luma => 2,
            ChunkType.Rgb => 4,
            ChunkType.Rgba => 5,
            _ => throw new ArgumentException($"Unknown chunk type : {chunkType}", nameof(chunkType)),
        };
}