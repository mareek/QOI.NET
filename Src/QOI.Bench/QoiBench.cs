using System.Drawing;
using BenchmarkDotNet.Attributes;

namespace QOI.Bench;

public class QoiBench
{
    private readonly byte[] _kodim23QoiImageData;
    private readonly byte[] _diceQoiImageData;
    private readonly byte[] _qoilogoQoiImageData;
    private readonly byte[] _kodim23PngImageData;
    private readonly byte[] _dicePngImageData;
    private readonly byte[] _qoilogoPngImageData;

    public QoiBench()
    {
        _kodim23QoiImageData = File.ReadAllBytes(@"TestImages\kodim23.qoi");
        _diceQoiImageData = File.ReadAllBytes(@"TestImages\dice.qoi");
        _qoilogoQoiImageData = File.ReadAllBytes(@"TestImages\qoi_logo.qoi");
        _kodim23PngImageData = File.ReadAllBytes(@"TestImages\kodim23.png");
        _dicePngImageData = File.ReadAllBytes(@"TestImages\dice.png");
        _qoilogoPngImageData = File.ReadAllBytes(@"TestImages\qoi_logo.png");
    }

    private readonly RawDecoder _rawDecoder = new();

    [Benchmark]
    public void DecodeKodim23Image() => _rawDecoder.Read(_kodim23QoiImageData);

    [Benchmark]
    public void DecodeDiceImage() => _rawDecoder.Read(_diceQoiImageData);

    [Benchmark]
    public void DecodeQoiLogoImage() => _rawDecoder.Read(_qoilogoQoiImageData);

    [Benchmark]
    public void DecodeKodim23PngImage() => Image.FromStream(new MemoryStream(_kodim23PngImageData));

    [Benchmark]
    public void DecodeDicePngImage() => Image.FromStream(new MemoryStream(_dicePngImageData));

    [Benchmark]
    public void DecodeQoiLogoPngImage() => Image.FromStream(new MemoryStream(_qoilogoPngImageData));
}
