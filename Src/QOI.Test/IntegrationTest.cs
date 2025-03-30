using System.Drawing;
using NFluent;
using QOI.Core.Debugging;
using QOI.Gdi;
using Xunit;

namespace QOI.Test;

public class IntegrationTest : IntegrationTestBase
{
    [Theory]
    [MemberData(nameof(GetReferenceImageCouples))]
    public void DecoderStandardImagesTest(string pngFilePath, string qoiFilePath)
    {
        var referenceImage = new Bitmap(pngFilePath);

        QoiBitmapDecoder decoder = new();
        var qoiImage = decoder.Read(qoiFilePath);

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
    public void EncoderStandardImagesTest(string pngFilePath, string qoiFilePath)
    {
        QoiFileAnalyzer analyzer = new();

        MemoryStream encodedImageStream = new();
        Bitmap pngImage = new(pngFilePath);
        QoiBitmapEncoder encoder = new();
        encoder.Write(pngImage, encodedImageStream);

        encodedImageStream.Position = 0;
        var encodedImageInfo = analyzer.AnalyzeFile($"Encoded {Path.GetFileName(pngFilePath)}", encodedImageStream);
        var encodedImageReport = encodedImageInfo.GetSummary(false);

        var referenceImageInfo = analyzer.AnalyzeFile(qoiFilePath);
        var referenceImageReport = referenceImageInfo.GetSummary(false);

        Check.That(encodedImageReport).IsEqualTo(referenceImageReport);
    }
}