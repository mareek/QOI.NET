using QOI.Core.Debugging;
using QOI.Encoder;
using QOI.ImageSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;


if (args.Length < 2)
{
    OutputDoc();
    return;
}

ArgumentsValidator argsValidator = new(args);

if (!argsValidator.Validate(out var errorMessage))
{
    Console.WriteLine(errorMessage);
    return;
}

switch (argsValidator.Command)
{
    case CommandType.Analyze:
        AnalyzeImage(argsValidator.SourceFilePath);
        break;
    case CommandType.Encode:
        EncodeImage(argsValidator.SourceFilePath, argsValidator.DestFilePath);
        break;
    case CommandType.Decode:
        DecodeImage(argsValidator.SourceFilePath, argsValidator.DestFilePath);
        break;
}


static void OutputDoc()
{
    Console.WriteLine("encode/decode sourceFile [targetFile]");
}

static void AnalyzeImage(string imagePath)
{
    var fileInfo = new QoiFileAnalyzer().AnalyzeFile(imagePath);
    Console.WriteLine(fileInfo.GetSummary(true));
}

static void EncodeImage(string sourceFilePath, string destFilePath)
{
    DeleteFileIfExists(destFilePath);

    var sourceFileData = File.ReadAllBytes(sourceFilePath);
    QoiImageSharpEncoder encoder = new();

    using var destFileStream = File.Create(destFilePath);

    if (Image.Identify(sourceFileData).PixelType.BitsPerPixel == 24)
        encoder.Write(Image.Load<Rgb24>(sourceFileData), destFileStream);
    else
        encoder.Write(Image.Load<Rgba32>(sourceFileData), destFileStream);
}

static void DecodeImage(string sourceFilePath, string destFilePath)
{
    DeleteFileIfExists(destFilePath);

    var bmpImage = new QoiImageSharpDecoder().Read(sourceFilePath);
    using var fileStream = File.OpenWrite(destFilePath);
    bmpImage.Save(fileStream, new SixLabors.ImageSharp.Formats.Png.PngEncoder());
}

static void DeleteFileIfExists(string filePath)
{
    if (File.Exists(filePath))
    {
        File.Delete(filePath);
    }
}
