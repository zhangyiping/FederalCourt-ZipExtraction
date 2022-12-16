namespace ZipExtraction.Validators;

public interface IZipContentValidator
{
    bool ValidateZipContent(string zipPath);
}