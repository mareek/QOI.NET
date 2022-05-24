using NFluent;
using Xunit;

namespace QOI.Core.Test;

public class BaseDecoderTest
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
}
