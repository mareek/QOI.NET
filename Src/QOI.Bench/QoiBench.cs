using System.Drawing;
using System.Drawing.Imaging;
using BenchmarkDotNet.Attributes;
using QOI.Core;

namespace QOI.Bench;

public class QoiBench
{
    private readonly BenchHelper _diceBenchHelper = new(@"TestImages\dice");
    private readonly BenchHelper _kodim23BenchHelper = new(@"TestImages\kodim23");
    private readonly BenchHelper _qoiLogoBenchHelper = new(@"TestImages\qoi_logo");


    [Benchmark]
    public void DecodeKodim23QoiImage() => _kodim23BenchHelper.DecodeQoi();

    [Benchmark]
    public void DecodeDiceQoiImage() => _diceBenchHelper.DecodeQoi();

    [Benchmark]
    public void DecodeQoiLogoImage() => _qoiLogoBenchHelper.DecodeQoi();

    [Benchmark]
    public void DecodeKodim23PngImage() => _kodim23BenchHelper.DecodePng();

    [Benchmark]
    public void DecodeDicePngImage() => _diceBenchHelper.DecodePng();

    [Benchmark]
    public void DecodeQoiLogoPngImage() => _qoiLogoBenchHelper.DecodePng();

    [Benchmark]
    public void EncodeKodim23Image() => _kodim23BenchHelper.EncodeImageToQoi();

    [Benchmark]
    public void EncodeDiceImage() => _diceBenchHelper.EncodeImageToQoi();

    [Benchmark]
    public void EncodeQoiLogoImage() => _qoiLogoBenchHelper.EncodeImageToQoi();

    [Benchmark]
    public void EncodeKodim23PngImage() => _kodim23BenchHelper.EncodeImageToPng();

    [Benchmark]
    public void EncodeDicePngImage() => _diceBenchHelper.EncodeImageToPng();

    [Benchmark]
    public void EncodeQoiLogoPngImage() => _qoiLogoBenchHelper.EncodeImageToPng();

    public class BenchHelper
    {
        private readonly QoiDecoder _qoiDecoder = new();
        private readonly QoiEncoder _qoiEncoder = new();

        public BenchHelper(string baseImagePath)
        {
            QoiImageData = File.ReadAllBytes(baseImagePath + ".qoi");
            PngImageData = File.ReadAllBytes(baseImagePath + ".png");

            BitmapImage = new Bitmap(new MemoryStream(PngImageData));

            Width = (uint)BitmapImage.Width;
            Height = (uint)BitmapImage.Height; ;

            Pixels = new QoiColor[BitmapImage.Width * BitmapImage.Height];
            for (int y = 0; y < BitmapImage.Height; y++)
                for (int x = 0; x < BitmapImage.Width; x++)
                {
                    var color = BitmapImage.GetPixel(x, y);
                    Pixels[y * BitmapImage.Width + x] = QoiColor.FromArgb(color.A, color.R, color.G, color.B);
                }

        }

        public byte[] QoiImageData { get; }
        public byte[] PngImageData { get; }
        public Bitmap BitmapImage { get; }
        public uint Width { get; }
        public uint Height { get; }
        public QoiColor[] Pixels { get; }

        public void EncodeImageToPng() => BitmapImage.Save(new ForgetStream(), ImageFormat.Png);

        public void EncodeImageToQoi() => _qoiEncoder.Write(Width, Height, true, true, Pixels, new ForgetStream());

        public void DecodeQoi() => _qoiDecoder.Read(new MemoryStream(QoiImageData));

        public void DecodePng() => Image.FromStream(new MemoryStream(PngImageData));

    }
}
