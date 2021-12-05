using System.Drawing;
using NFluent;
using QOI.NET;
using Xunit;

namespace QOI.Test;

public class IntegrationTest
{
    private static readonly Color[] SimpleColors =
    {
        Color.Chartreuse,
        Color.Bisque,
        Color.Aquamarine,
        Color.OrangeRed,
        Color.Crimson,
        Color.LemonChiffon,
        Color.PeachPuff,
        Color.AntiqueWhite,
        Color.BlanchedAlmond,
        Color.CornflowerBlue,
        Color.BurlyWood,
        Color.Firebrick,
        Color.RosyBrown,
        Color.DeepSkyBlue,
    };

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

    public static IEnumerable<object[]> GetImgTest()
    {
        Bitmap blankImage = new(8, 6);
        yield return new object[] { blankImage };
        yield return new object[] { GetRandomStripedImage() };
        yield return new object[] { GetRandomSimpleImage() };
        yield return new object[] { GetRandomArgbImage() };
    }

    private static Bitmap GetRandomStripedImage()
    {
        Bitmap stripedImage = new(8, 6);
        for (int y = 0; y < stripedImage.Height; y++)
        {
            var color = SimpleColors[Random.Shared.Next(0, SimpleColors.Length)];
            for (int x = 0; x < stripedImage.Width; x++)
            {
                stripedImage.SetPixel(x, y, color);
            }
        }
        return stripedImage;
    }

    private static Bitmap GetRandomSimpleImage()
    {
        Bitmap simpleImage = new(8, 6);
        for (int y = 0; y < simpleImage.Height; y++)
        {
            for (int x = 0; x < simpleImage.Width; x++)
            {
                var color = SimpleColors[Random.Shared.Next(0, SimpleColors.Length)];
                simpleImage.SetPixel(x, y, color);
            }
        }
        return simpleImage;
    }

    private static Bitmap GetRandomArgbImage()
    {
        Bitmap simpleImage = new(8, 6);
        for (int y = 0; y < simpleImage.Height; y++)
        {
            for (int x = 0; x < simpleImage.Width; x++)
            {
                var color = Color.FromArgb(Random.Shared.Next(0, 256),
                                           Random.Shared.Next(0, 256),
                                           Random.Shared.Next(0, 256),
                                           Random.Shared.Next(0, 256));
                simpleImage.SetPixel(x, y, color);
            }
        }
        return simpleImage;
    }
}