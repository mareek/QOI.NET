using NFluent;
using QOI.Core.Debugging;
using QOI.ImageSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Xunit;

namespace QOI.Test;

public class ImageSharpTest : IntegrationTestBase
{
    [Theory]
    [MemberData(nameof(GetReferenceImageCouples))]
    public void DecoderImageSharpStandardImagesTest(FileInfo pngFile, FileInfo qoiFile)
    {
        var imageInfo = Image.Identify(pngFile.FullName);
        if (imageInfo.PixelType.BitsPerPixel == 24)
            CompareOriginalToDecoded<Rgb24>(pngFile, qoiFile);
        else
            CompareOriginalToDecoded<Rgba32>(pngFile, qoiFile);
    }

    private static void CompareOriginalToDecoded<TPixel>(FileInfo pngFile, FileInfo qoiFile)
        where TPixel : unmanaged, IPixel<TPixel>
    {

        var referenceImage = Image.Load<TPixel>(pngFile.FullName);

        QoiImageSharpDecoder decoder = new();
        var qoiImage = (Image<TPixel>)decoder.Read(qoiFile.FullName);

        for (int y = 0; y < referenceImage.Height; y++)
        {
            for (int x = 0; x < referenceImage.Width; x++)
            {
                var pixelReference = referenceImage[x, y];
                var pixelQoi = qoiImage[x, y];
                Check.That(pixelQoi).IsEqualTo(pixelReference);
            }
        }
    }

    [Theory]
    [MemberData(nameof(GetReferenceImageCouples))]
    public void EncoderImageSharpStandardImagesTest(FileInfo pngFile, FileInfo qoiFile)
    {
        QoiFileAnalyzer analyzer = new();

        MemoryStream encodedImageStream = new();
        QoiImageSharpEncoder encoder = new();

        var imageInfo = Image.Identify(pngFile.FullName);
        if (imageInfo.PixelType.BitsPerPixel == 24)
            encoder.Write(Image.Load<Rgb24>(pngFile.FullName), encodedImageStream);
        else
            encoder.Write(Image.Load<Rgba32>(pngFile.FullName), encodedImageStream);

        encodedImageStream.Position = 0;
        var encodedImageInfo = analyzer.AnalyzeFile($"Encoded {pngFile.Name}", encodedImageStream);
        var encodedImageReport = encodedImageInfo.GetSummary(false);

        var referenceImageInfo = analyzer.AnalyzeFile(qoiFile.FullName);
        var referenceImageReport = referenceImageInfo.GetSummary(false);

        Check.That(encodedImageReport).IsEqualTo(referenceImageReport);
    }
}
