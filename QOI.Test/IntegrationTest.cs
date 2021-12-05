using System.Drawing;
using NFluent;
using QOI.NET;
using Xunit;

namespace QOI.Test;

public class IntegrationTest
{
    [Theory]
    [MemberData(nameof(GetImgTest))]
    public void DoMyTest(Bitmap imgTest)
    {

        var encoder = new QoiEncoder();
        var imgBytes = encoder.Write(imgTest);
        
        var decodeStream = new MemoryStream(imgBytes);
        var decoder = new QoiDecoder();
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
        const int width = 8;
        const int height = 6;

        yield return new object[] { new Bitmap(width, height) };
    }

    private void SetRandomPixels(Bitmap image)
    {
        
    }
}