using System.IO.Compression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Serilog;
using ZipExtraction.Configurations;
using ZipExtraction.Exceptions;

namespace ZipExtraction.Validators;

public class ZipContentValidator: IZipContentValidator
{
    private readonly ILogger _logger;

    public ZipContentValidator(ILogger logger)
    {
        _logger = logger;
    }

    public bool ValidateZipContent(string zipPath)
    {

        _logger.Information("Hello");

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
        foreach (var fileName in fullFileNames)
        {
            var extension = Path.GetExtension(fileName);
            if (!validFileExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
            {
                throw new InvalidFileTypeException(
                    $"Invalid file found in zip. File name: {fileName}");
            }
        }

        return true;
    }
}