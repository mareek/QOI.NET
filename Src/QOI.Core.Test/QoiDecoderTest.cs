using NFluent;
using Xunit;

namespace QOI.Core.Test;

public class QoiDecoderTest
{
    public static object[][] GetFileValidityTestSet()
        => new object[][]
        {
            new object[] { false, Array.Empty<byte>() },
            new object[] { false, new byte[] { 0, 0, 0, 0, 0, 0 } },
            new object[] { false, new byte[] { 255, 38, 252, 55, 127, 0, 0, 0 } },
            new object[] { false, new byte[] { 0x71, 0x6F } },
            new object[] { false, new byte[] { 0x71, 0x6F, 0x69, 0xFF, 0x12 } },
            new object[] { true, new byte[] { 0x71, 0x6F, 0x69, 0x66 } },
            new object[] { true, new byte[] { 0x71, 0x6F, 0x69, 0x66, 1, 2, 3, 4 } },
        };

    [Theory]
    [MemberData(nameof(GetFileValidityTestSet))]
    public void InvalidImages(bool isValid, params byte[] imageData)
    {
        using MemoryStream stream = new(imageData);
        Check.That(QoiDecoder.IsQoiImage(stream)).IsEqualTo(isValid);
    }

    [Theory]
    [MemberData(nameof(GetImageTestSet))]
    public void FullCircleProcessTest(QoiImage imgTest)
    {
        using var stream = new MemoryStream();
        new QoiEncoder().Write(imgTest.Width, imgTest.Height, imgTest.HasAlpha, imgTest.IsSrgb, imgTest.Pixels, stream);
        var imgBytes = stream.ToArray();

        using MemoryStream decodeStream = new(imgBytes);
        QoiDecoder decoder = new();
        var decodedImage = decoder.Read(decodeStream);

        Check.That(imgTest.Height).IsEqualTo(decodedImage.Height);
        Check.That(imgTest.Width).IsEqualTo(decodedImage.Width);
        Check.That(imgTest.IsSrgb).IsEqualTo(decodedImage.IsSrgb);
        Check.That(imgTest.HasAlpha).IsEqualTo(decodedImage.HasAlpha);
        Check.That(imgTest.Pixels.Length).IsEqualTo(decodedImage.Pixels.Length);

        for (int pixelIndex = 0; pixelIndex < imgTest.Pixels.Length; pixelIndex++)
        {
            var pixelSrc = imgTest.Pixels[pixelIndex];
            var pixelDest = decodedImage.Pixels[pixelIndex];
            Check.That(pixelDest).IsEqualTo(pixelSrc);
        }
    }

    public static IEnumerable<QoiImage[]> GetImageTestSet()
    {
        yield return new[] { GetBlankImage() };
        yield return new[] { GetRandomArgbImage() };
        yield return new[] { GetMonochromeStripedImage() };
        yield return new[] { GetRandomStripedImage() };
        yield return new[] { GetRandomSimpleImage() };
        yield return new[] { GetShadedImage() };
        yield return new[] { GetShadedImage(3) };
    }

    private static QoiImage GetBlankImage(int width = 8, int height = 6)
        => NewQoiImage(width, height, new QoiColor[width * height]);

    private static QoiImage GetRandomArgbImage(int width = 8, int height = 6)
    {
        QoiColor[] pixels = new QoiColor[width * height];
        for (int pixelIndex = 0; pixelIndex < pixels.Length; pixelIndex++)
        {
            pixels[pixelIndex] = GetRandomColor();
        }

        return NewQoiImage(width, height, pixels);

    }

    private static QoiImage GetMonochromeStripedImage(int width = 8, int height = 6)
    {
        QoiColor[] colors = { GetRandomColor(), GetRandomColor() };
        QoiColor[] pixels = new QoiColor[width * height];
        int pixelIndex = 0;
        for (int y = 0; y < height; y++)
        {
            var color = colors[y % 2];
            for (int x = 0; x < width; x++)
            {
                pixels[pixelIndex++] = color;
            }
        }

        return NewQoiImage(width, height, pixels);
    }

    private static QoiImage GetRandomStripedImage(int width = 8, int height = 6)
    {
        QoiColor[] pixels = new QoiColor[width * height];
        int pixelIndex = 0;
        for (int y = 0; y < height; y++)
        {
            QoiColor color = GetRandomColor();
            for (int x = 0; x < width; x++)
            {
                pixels[pixelIndex++] = color;
            }
        }
        return NewQoiImage(width, height, pixels);
    }

    private static QoiImage GetRandomSimpleImage(int width = 8, int height = 6)
    {
        QoiColor[] colors = Enumerable.Range(0, 16).Select(_ => GetRandomColor()).ToArray();

        QoiColor[] pixels = new QoiColor[width * height];
        for (int pixelIndex = 0; pixelIndex < pixels.Length; pixelIndex++)
        {
            pixels[pixelIndex] = colors[Random.Shared.Next(0, colors.Length)];
        }

        return NewQoiImage(width, height, pixels);
    }

    private static QoiImage GetShadedImage(byte increment = 1, int width = 8, int height = 6)
    {
        QoiColor color = QoiColor.FromArgb(255, 0, 0, 0);
        QoiColor[] pixels = new QoiColor[width * height];
        for (int pixelIndex = 0; pixelIndex < pixels.Length; pixelIndex++)
        {
            pixels[pixelIndex] = color;
            color = QoiColor.FromArgb(color.A,
                                      (byte)(color.R + increment),
                                      (byte)(color.G + increment),
                                      (byte)(color.B + increment));
        }

        return NewQoiImage(width, height, pixels);
    }

    private static QoiImage NewQoiImage(int width, int height, QoiColor[] pixels)
        => new((uint)width, (uint)height, true, true, pixels);

    private static QoiColor GetRandomColor()
    {
        Span<byte> bytes = stackalloc byte[4];
        Random.Shared.NextBytes(bytes);
        return QoiColor.FromArgb(bytes[0], bytes[1], bytes[2], bytes[3]);
    }

    /*
    TODO:
    - MAJ le package nuget QOI.Core en le passant en 2.0.0 (breaking change)
    - Mettre les autres projets sur nuget
    - rapatrier des tests ici
    - profiler le code pour voir s'il y a des gains de perfs facilement atteignables
    - faire un Encoder/decoder en ligne de commande et le publier sur NuGet
    - réfléchir si on peut faire un rendre le decoder streamable (sans alouer un gros tableau)
     */
}
