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
        var encodedImageReport = encodedImageInfo.GetSummary(false);

        var referenceImageInfo = analyzer.AnalyzeFile(qoiFile.FullName);
        var referenceImageReport = referenceImageInfo.GetSummary(false);

        Check.That(encodedImageReport).IsEqualTo(referenceImageReport);
    }
}