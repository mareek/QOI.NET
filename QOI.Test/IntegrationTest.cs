﻿using System.Drawing;
using NFluent;
using QOI.NET;
using Xunit;

namespace QOI.Test;

public class IntegrationTest
{
    [Theory]
    [MemberData(nameof(GetImgTest))]
    public void FullCircleProcessTest(Bitmap imgTest)
    {
        QoiEncoder encoder = new();
        var imgBytes = encoder.Write(imgTest);

        using MemoryStream decodeStream = new(imgBytes);
        QoiDecoder decoder = new();
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

    [Fact]
    public void TestCompressedSize()
    {
        const int HeaderLength = 14;
        const int colorChunkLength = 5;
        const int run8ChunkLength = 1;
        const int indexChunkLength = 1;

        QoiEncoder encoder = new();
        Check.That(encoder.Write(GetBlankImage())).HasSize(HeaderLength + colorChunkLength + run8ChunkLength * 2);
        Check.That(encoder.Write(GetRandomArgbImage())).HasSize(HeaderLength + colorChunkLength * 8 * 6);
        Check.That(encoder.Write(GetRandomStripedImage())).HasSize(HeaderLength + (colorChunkLength + run8ChunkLength) * 6);
        Check.That(encoder.Write(GetMonochromeStripedImage())).HasSize(HeaderLength + colorChunkLength * 2 + indexChunkLength * 4 + 6 * run8ChunkLength);
    }

    public static IEnumerable<object[]> GetImgTest()
    {
        yield return new object[] { GetBlankImage() };
        yield return new object[] { GetRandomArgbImage() };
        yield return new object[] { GetMonochromeStripedImage() };
        yield return new object[] { GetRandomStripedImage() };
        yield return new object[] { GetRandomSimpleImage() };
    }

    private static Bitmap GetBlankImage() => new(8, 6);

    private static Bitmap GetRandomArgbImage()
    {
        Bitmap image = new(8, 6);
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

    private static Bitmap GetMonochromeStripedImage()
    {
        Color[] colors = { Color.Bisque, Color.Chartreuse };
        Bitmap image = new(8, 6);
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

    private static Bitmap GetRandomStripedImage()
    {
        Bitmap image = new(8, 6);
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

    private static Bitmap GetRandomSimpleImage()
    {
        Color[] colors = Enumerable.Range(0, 16).Select(_ => GetRandomColor()).ToArray();

        Bitmap image = new(8, 6);
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

    private static Color GetRandomColor()
        => Color.FromArgb(Random.Shared.Next(0, 256),
                          Random.Shared.Next(0, 256),
                          Random.Shared.Next(0, 256),
                          Random.Shared.Next(0, 256));
}