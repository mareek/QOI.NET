﻿using System.Drawing;
using NFluent;
using QOI.NET;
using Xunit;

namespace QOI.Test;

public class IntegrationTest
{
    [Theory]
    [MemberData(nameof(GetImageTestSet))]
    public void FullCircleProcessTest(Bitmap imgTest)
    {
        QoiBitmapEncoder encoder = new();
        var imgBytes = encoder.Write(imgTest);

        using MemoryStream decodeStream = new(imgBytes);
        QoiBitmapDecoder decoder = new();
        var decodedImage = decoder.Read(decodeStream);

        for (int y = 0; y < imgTest.Height; y++)
        {
            for (int x = 0; x < imgTest.Width; x++)
            {
                var pixelSrc = imgTest.GetPixel(x, y);
                var pixelDest = decodedImage.GetPixel(x, y);
                Check.That(pixelSrc).IsEqualTo(pixelDest);
            }
        }
    }

    public static IEnumerable<object[]> GetImageTestSet()
    {
        yield return new object[] { GetBlankImage() };
        yield return new object[] { GetRandomArgbImage() };
        yield return new object[] { GetMonochromeStripedImage() };
        yield return new object[] { GetRandomStripedImage() };
        yield return new object[] { GetRandomSimpleImage() };
        yield return new object[] { GetShadedImage() };
    }

    [Theory]
    [MemberData(nameof(GetSizeTestSet))]
    public void TestCompressedSize(Bitmap image, int compressedSize)
    {
        QoiBitmapEncoder encoder = new();
        Check.That(encoder.Write(image)).HasSize(compressedSize);
    }

    public static IEnumerable<object[]> GetSizeTestSet()
    {
        const int HeaderLength = 14;
        const int colorChunkLength = 5;
        const int run8ChunkLength = 1;
        const int run16ChunkLength = 2;
        const int indexChunkLength = 1;
        const int diff8ChunkLength = 1;

        yield return new object[] { GetBlankImage(80, 60), HeaderLength + colorChunkLength + run16ChunkLength };
        yield return new object[] { GetRandomArgbImage(), HeaderLength + colorChunkLength * 8 * 6 };
        yield return new object[] { GetShadedImage(), HeaderLength + colorChunkLength + diff8ChunkLength * (8 * 6 - 1) };
        yield return new object[] { GetRandomStripedImage(), HeaderLength + (colorChunkLength + run8ChunkLength) * 6 };
        yield return new object[] { GetMonochromeStripedImage(), HeaderLength
                                                                 + colorChunkLength * 2
                                                                 + indexChunkLength * 4
                                                                 + run8ChunkLength * 6 };
    }

    private static Bitmap GetBlankImage(int width = 8, int height = 6) => new(width, height);

    private static Bitmap GetRandomArgbImage(int width = 8, int height = 6)
    {
        Bitmap image = new(width, height);
        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                var color = GetRandomColor();
                image.SetPixel(x, y, color);
            }
        }
        return image;
    }

    private static Bitmap GetMonochromeStripedImage(int width = 8, int height = 6)
    {
        Color[] colors = { Color.Bisque, Color.Chartreuse };
        Bitmap image = new(width, height);
        for (int y = 0; y < image.Height; y++)
        {
            var color = colors[y % 2];
            for (int x = 0; x < image.Width; x++)
            {
                image.SetPixel(x, y, color);
            }
        }
        return image;
    }

    private static Bitmap GetRandomStripedImage(int width = 8, int height = 6)
    {
        Bitmap image = new(width, height);
        for (int y = 0; y < image.Height; y++)
        {
            Color color = GetRandomColor();
            for (int x = 0; x < image.Width; x++)
            {
                image.SetPixel(x, y, color);
            }
        }
        return image;
    }

    private static Bitmap GetRandomSimpleImage(int width = 8, int height = 6)
    {
        Color[] colors = Enumerable.Range(0, 16).Select(_ => GetRandomColor()).ToArray();

        Bitmap image = new(width, height);
        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                var color = colors[Random.Shared.Next(0, colors.Length)];
                image.SetPixel(x, y, color);
            }
        }
        return image;
    }

    private static Bitmap GetShadedImage(int width = 8, int height = 6)
    {
        Bitmap image = new(width, height);
        Color color = Color.Black;
        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                image.SetPixel(x, y, color);
                color = Color.FromArgb(color.A, color.R + 1, color.G + 1, color.B + 1);
            }
        }
        return image;
    }

    private static Color GetRandomColor()
        => Color.FromArgb(Random.Shared.Next(0, 256),
                          Random.Shared.Next(0, 256),
                          Random.Shared.Next(0, 256),
                          Random.Shared.Next(0, 256));
}