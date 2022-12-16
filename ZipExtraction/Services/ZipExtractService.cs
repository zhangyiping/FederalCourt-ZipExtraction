using System.IO.Compression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ZipExtraction.Exceptions;
using ZipExtraction.Validators;

namespace ZipExtraction.Services;

public class ZipExtractService
{
    private readonly IZipContentValidator _zipContentValidator;
    private readonly IXmlValidator _xmlValidator;
    private readonly IEmailService _emailService;

    public ZipExtractService(IZipContentValidator zipContentValidator, IXmlValidator xmlValidator, IEmailService emailService)
    {
        _zipContentValidator = zipContentValidator;
        _xmlValidator = xmlValidator;
        _emailService = emailService;
    }

    public async Task ExtractZip(string zipPath)
    {
        try
        {
            _zipContentValidator.ValidateZipContent(zipPath);

            // TODO instantiate a Configuration in Startup. Then, use the Options pattern to access individual settings
            var subPath = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetValue<string>("ExtractedFolderLocation");
            var temporarySubPath = subPath + "Temp";
            var temporaryExtractPath= Environment.ExpandEnvironmentVariables(temporarySubPath);
            ZipFile.ExtractToDirectory(zipPath, temporaryExtractPath);

            var applicationNumber = _xmlValidator.ValidateXml();
            var finalDestination = Environment.ExpandEnvironmentVariables(subPath)+ applicationNumber;
            Directory.Move(temporaryExtractPath, finalDestination);
            //TODO more zip file for the same application number

            //TODO compose successful email 
            await _emailService.SendEmail();
        }
        catch (InvalidFileTypeException exception)
        {
            //TODO compose error email: unsupported file type is included in zip
            await _emailService.SendEmail();
        }
        catch (MissingPartyXmlException exception)
        {
            //TODO compose error email: mandatory party.xml file is missing 
            await _emailService.SendEmail();
        }
        catch (InvalidDataException exception)
        {
            //TODO compose error email: zip is corrupted
            await _emailService.SendEmail();
        }
        catch (Exception exception)
        {
            //TODO compose error email: something went horribly horribly wrong
            await _emailService.SendEmail();
        }
    }
}