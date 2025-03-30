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
    public void DecoderImageSharpStandardImagesTest(string pngFilePath, string qoiFilePath)
    {
        var imageInfo = Image.Identify(pngFilePath);
        if (imageInfo.PixelType.BitsPerPixel == 24)
            CompareOriginalToDecoded<Rgb24>(pngFilePath, qoiFilePath);
        else
            CompareOriginalToDecoded<Rgba32>(pngFilePath, qoiFilePath);
    }

    private static void CompareOriginalToDecoded<TPixel>(string pngFilePath, string qoiFilePath)
        where TPixel : unmanaged, IPixel<TPixel>
    {

        var referenceImage = Image.Load<TPixel>(pngFilePath);

        QoiImageSharpDecoder decoder = new();
        var qoiImage = (Image<TPixel>)decoder.Read(qoiFilePath);

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
    public void EncoderImageSharpStandardImagesTest(string pngFilePath, string qoiFilePath)
    {
        QoiFileAnalyzer analyzer = new();

        MemoryStream encodedImageStream = new();
        QoiImageSharpEncoder encoder = new();

        var imageInfo = Image.Identify(pngFilePath);
        if (imageInfo.PixelType.BitsPerPixel == 24)
            encoder.Write(Image.Load<Rgb24>(pngFilePath), encodedImageStream);
        else
            encoder.Write(Image.Load<Rgba32>(pngFilePath), encodedImageStream);

        encodedImageStream.Position = 0;
        var encodedImageInfo = analyzer.AnalyzeFile($"Encoded {Path.GetFileName(pngFilePath)}", encodedImageStream);
        var encodedImageReport = encodedImageInfo.GetSummary(false);

        var referenceImageInfo = analyzer.AnalyzeFile(qoiFilePath);
        var referenceImageReport = referenceImageInfo.GetSummary(false);

        Check.That(encodedImageReport).IsEqualTo(referenceImageReport);
    }
}
