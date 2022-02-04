using System.Drawing;
using QOI.Core.Debugging;
using QOI.Gdi;

namespace QOI.Encoder;

public class Program
{
    static void Main(string[] args)
    {
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
    }

    private static void OutputDoc()
    {
        Console.WriteLine("encode/decode sourceFile [targetFile]");
    }

    private static void AnalyzeImage(string imagePath)
    {
        var fileInfo = new QoiFileAnalyzer().AnalyzeFile(new FileInfo(imagePath));
        Console.WriteLine(fileInfo.GetSummary());
    }

    private static void EncodeImage(string sourceFilePath, string destFilePath)
    {
        DeleteFileIfExists(destFilePath);
        Bitmap sourceImage = new(sourceFilePath);
        using var destFileStream = File.Create(destFilePath);
        new QoiBitmapEncoder().Write(sourceImage, destFileStream);
    }

    static void DecodeImage(string sourceFilePath, string destFilePath)
    {
        DeleteFileIfExists(destFilePath);
        var bmpImage = new QoiBitmapDecoder().Read(sourceFilePath);
        bmpImage.Save(destFilePath);
    }

    private static void DeleteFileIfExists(string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
}
