using System.Drawing;
using NFluent;
using QOI.NET;
using Xunit;

namespace QOI.Test;

public class IntegrationTest
{
    [Theory]
    [InlineData("testcard.png", "testcard.qoi")]
    [InlineData("testcard_rgba.png", "testcard_rgba.qoi")]
    [InlineData("dice.png", "dice.qoi")]
    public void DecoderStandardImagesTest(string referenceFileName, string qoiFileName)
    {
        var referenceImage = new Bitmap(Path.Combine("TestImages", referenceFileName));

        QoiBitmapDecoder decoder = new();
        var qoiImage = decoder.Read(Path.Combine("TestImages", qoiFileName));

        for (int y = 0; y < referenceImage.Height; y++)
        {
            for (int x = 0; x < referenceImage.Width; x++)
            {
                var pixelReference = referenceImage.GetPixel(x, y);
                var pixelQoi = qoiImage.GetPixel(x, y);
                Check.That(pixelQoi).IsEqualTo(pixelReference);
            }
        }
    }

    [Theory]
    [MemberData(nameof(GetImageTestSet))]
    public void FullCircleProcessTest(Bitmap imgTest)
    {
        using var stream = new MemoryStream();
        new QoiBitmapEncoder().Write(imgTest, stream);
        var imgBytes = stream.ToArray();

        using MemoryStream decodeStream = new(imgBytes);
        QoiBitmapDecoder decoder = new();
        var decodedImage = decoder.Read(decodeStream);

        for (int y = 0; y < imgTest.Height; y++)
        {
            for (int x = 0; x < imgTest.Width; x++)
            {
                var pixelSrc = imgTest.GetPixel(x, y);
                var pixelDest = decodedImage.GetPixel(x, y);
                Check.That(pixelDest).IsEqualTo(pixelSrc);
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
        yield return new object[] { GetShadedImage(3) };
        yield return new object[] { new Bitmap(Path.Combine("TestImages", "testcard.png")) };
        yield return new object[] { new Bitmap(Path.Combine("TestImages", "testcard_rgba.png")) };
        yield return new object[] { new Bitmap(Path.Combine("TestImages", "dice.png")) };
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

    private static Bitmap GetShadedImage(int increment = 1, int width = 8, int height = 6)
    {
        Bitmap image = new(width, height);
        Color color = Color.Black;
        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                image.SetPixel(x, y, color);
                color = Color.FromArgb(color.A,
                                       color.R + increment,
                                       color.G + increment,
                                       color.B + increment);
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