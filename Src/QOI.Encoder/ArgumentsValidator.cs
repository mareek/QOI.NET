namespace QOI.Encoder;
internal class ArgumentsValidator
{
    private readonly string[] _args;

    public ArgumentsValidator(string[] args)
    {
        _args = args;
    }

    public CommandType Command => Enum.Parse<CommandType>(_args[0], true);

    public string SourceFilePath => _args[1];

    public string DestFilePath => DestFileDefined ? _args[2] : GetDestFilePath();

    private bool DestFileDefined => _args.Length > 2;

    public bool Validate(out string errorMessage)
    {
        if (!Enum.TryParse<CommandType>(_args[0], true, out var _))
        {
            errorMessage = $"Unkonwn command \"{_args[0]}\"";
            return false;
        }
        else if (!ValidateFilePath(SourceFilePath, out errorMessage))
        {
            return false;
        }
        else if (!File.Exists(SourceFilePath))
        {
            errorMessage = $"The File \"{SourceFilePath}\" doesn't exists.";
            return false;
        }
        else if (DestFileDefined && !ValidateFilePath(_args[2], out errorMessage))
        {
            return false;
        }

        return true;
    }

    private bool ValidateFilePath(string filePath, out string errorMessage)
    {
        try
        {
            new FileInfo(filePath);
            errorMessage = string.Empty;
            return true;
        }
        catch
        {
            errorMessage = $"Invalid file path : \"{filePath}\"";
            return false;
        }
    }

    private string GetDestFilePath()
    {
        string extension = Command == CommandType.Encode ? "qoi" : "png";
        var destFileName = $"{Path.GetFileNameWithoutExtension(SourceFilePath)}.{extension}";
        return Path.Combine(Path.GetDirectoryName(SourceFilePath)!, destFileName);
    }
}
