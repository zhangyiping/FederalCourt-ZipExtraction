using System.IO.Compression;
using Microsoft.Extensions.Configuration;
using ZipExtraction.Exceptions;

namespace ZipExtraction.Validators;

public class ZipContentValidator: IZipContentValidator
{
    public void ValidateZipContent(string zipPath)
    {
        // TODO add all image file extensions
        // TODO instantiate a Configuration in Startup. Then, use the Options pattern to access individual settings
        
        var validFileExtensionsSeparatedByComma = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetValue<string>("ValidFileExtensions");
        var validFileExtensions = validFileExtensionsSeparatedByComma.Split(',').ToList();
        
        using var zipFile = ZipFile.OpenRead(zipPath);
        var zipArchiveEntries = zipFile.Entries;
        var fullFileNames = zipArchiveEntries.Select(x => x.FullName).ToList();
        if (!fullFileNames.Contains("party.XML", StringComparer.OrdinalIgnoreCase))
        {
            throw new MissingPartyXmlException("party.XML file is not included in uploaded zip");
        }
        foreach (var entry in zipArchiveEntries)
        {
            var extension = Path.GetExtension(entry.FullName);
            if (!validFileExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
            {
                throw new InvalidFileTypeException(
                    $"Invalid file found in zip. File name: {entry.FullName}");
            }
        }
    }
}