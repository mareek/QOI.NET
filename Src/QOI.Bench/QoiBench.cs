using System.Drawing;
using System.Drawing.Imaging;
using BenchmarkDotNet.Attributes;
using QOI.Core;

namespace QOI.Bench;

public class QoiBench
{
    private readonly byte[] _kodim23QoiImageData;
    private readonly byte[] _diceQoiImageData;
    private readonly byte[] _qoilogoQoiImageData;
    private readonly byte[] _kodim23PngImageData;
    private readonly byte[] _dicePngImageData;
    private readonly byte[] _qoilogoPngImageData;

    private readonly Bitmap _kodim23PngImage;
    private readonly Bitmap _dicePngImage;
    private readonly Bitmap _qoilogoPngImage;
    private readonly (uint width, uint height, QoiColor[] pixels) _kodim23PngImageInfo;
    private readonly (uint width, uint height, QoiColor[] pixels) _dicePngImageInfo;
    private readonly (uint width, uint height, QoiColor[] pixels) _qoilogoPngImageInfo;

    public QoiBench()
    {
        _kodim23QoiImageData = File.ReadAllBytes(@"TestImages\kodim23.qoi");
        _diceQoiImageData = File.ReadAllBytes(@"TestImages\dice.qoi");
        _qoilogoQoiImageData = File.ReadAllBytes(@"TestImages\qoi_logo.qoi");

        _kodim23PngImageData = File.ReadAllBytes(@"TestImages\kodim23.png");
        _dicePngImageData = File.ReadAllBytes(@"TestImages\dice.png");
        _qoilogoPngImageData = File.ReadAllBytes(@"TestImages\qoi_logo.png");

        _kodim23PngImage = new Bitmap(new MemoryStream(_kodim23PngImageData));
        _dicePngImage = new Bitmap(new MemoryStream(_dicePngImageData));
        _qoilogoPngImage = new Bitmap(new MemoryStream(_qoilogoPngImageData));

        _kodim23PngImageInfo = RawEncoder.GetImageData(_kodim23PngImage);
        _dicePngImageInfo = RawEncoder.GetImageData(_dicePngImage);
        _qoilogoPngImageInfo = RawEncoder.GetImageData(_qoilogoPngImage);
    }

    private readonly RawEncoder _rawEncoder = new();
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


    [Benchmark]
    public void EncodeKodim23Image()
        => _rawEncoder.Encode(_kodim23PngImageInfo.width, _kodim23PngImageInfo.height, _kodim23PngImageInfo.pixels);

    [Benchmark]
    public void EncodeDiceImage()
        => _rawEncoder.Encode(_dicePngImageInfo.width, _dicePngImageInfo.height, _dicePngImageInfo.pixels);

    [Benchmark]
    public void EncodeQoiLogoImage()
        => _rawEncoder.Encode(_qoilogoPngImageInfo.width, _qoilogoPngImageInfo.height, _qoilogoPngImageInfo.pixels);

    [Benchmark]
    public void EncodeKodim23PngImage() => _kodim23PngImage.Save(new ForgetStream(), ImageFormat.Png);

    [Benchmark]
    public void EncodeDicePngImage() => _dicePngImage.Save(new ForgetStream(), ImageFormat.Png);

    [Benchmark]
    public void EncodeQoiLogoPngImage() => _qoilogoPngImage.Save(new ForgetStream(), ImageFormat.Png);
}
