using System.Drawing;
using NFluent;
using QOI.Core.Debugging;
using QOI.NET;
using Xunit;

namespace QOI.Test;

public class IntegrationTest
{
    private static DirectoryInfo TestImagesDirectory => new("TestImages");

    [Theory]
    [MemberData(nameof(GetReferenceImageCouples))]
    public void DecoderStandardImagesTest(FileInfo pngFile, FileInfo qoiFile)
    {
        var referenceImage = new Bitmap(pngFile.FullName);

        QoiBitmapDecoder decoder = new();
        var qoiImage = decoder.Read(qoiFile.FullName);

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
    [MemberData(nameof(GetReferenceImageCouples))]
    public void EncoderStandardImagesTest(FileInfo pngFile, FileInfo qoiFile)
    {
        QoiFileAnalyzer analyzer = new();

        MemoryStream encodedImageStream = new();
        Bitmap pngImage = new(pngFile.FullName);
        QoiBitmapEncoder encoder = new();
        encoder.Write(pngImage, encodedImageStream);

        encodedImageStream.Position = 0;
        var encodedImageInfo = analyzer.AnalyzeFile($"Encoded {pngFile.Name}", encodedImageStream);
        var strEncoded = encodedImageInfo.GetDebugString(false);

        var referenceImageInfo = analyzer.AnalyzeFile(qoiFile);
        var strReference = referenceImageInfo.GetDebugString(false);

        Check.That(strEncoded).IsEqualTo(strReference);
    }

    public static IEnumerable<FileInfo[]> GetReferenceImageCouples()
    {
        var qoiFilesByName = TestImagesDirectory.EnumerateFiles("*.qoi")
                                                .ToDictionary(f => Path.GetFileNameWithoutExtension(f.FullName));
        var referencePngFiles = TestImagesDirectory.EnumerateFiles("*.png").ToArray();
        foreach (var referencePngFile in referencePngFiles)
        {
            var referenceQoiFile = qoiFilesByName[Path.GetFileNameWithoutExtension(referencePngFile.FullName)];
            yield return new[] { referencePngFile, referenceQoiFile };
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

    public static IEnumerable<Bitmap[]> GetImageTestSet()
    {
        yield return new[] { GetBlankImage() };
        yield return new[] { GetRandomArgbImage() };
        yield return new[] { GetMonochromeStripedImage() };
        yield return new[] { GetRandomStripedImage() };
        yield return new[] { GetRandomSimpleImage() };
        yield return new[] { GetShadedImage() };
        yield return new[] { GetShadedImage(3) };

        FileInfo[] referencePngFiles = TestImagesDirectory.EnumerateFiles("*.png").ToArray();
        foreach (var referencePngFile in referencePngFiles)
        {
            yield return new[] { new Bitmap(referencePngFile.FullName) };
        }
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