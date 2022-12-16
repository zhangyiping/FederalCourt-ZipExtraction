using System.IO.Compression;
using Microsoft.Extensions.Configuration;
using Serilog;
using ZipExtraction.Exceptions;
using ZipExtraction.Validators;

namespace ZipExtraction.Services;

public class ZipExtractService
{
    private readonly IZipContentValidator _zipContentValidator;
    private readonly IXmlValidator _xmlValidator;
    private readonly IEmailService _emailService;

    public ZipExtractService(IZipContentValidator zipContentValidator, 
        IXmlValidator xmlValidator, IEmailService emailService)
    {
        _zipContentValidator = zipContentValidator;
        _xmlValidator = xmlValidator;
        _emailService = emailService;
    }

    public async Task ExtractZip(string zipPath)
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();
        
        // TODO instantiate a Configuration in Startup. Then, use the Options pattern to access individual settings
        var partialDestinationPath = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetValue<string>("ExtractedFolderLocation");
        var temporarySubPath = partialDestinationPath + "Temp";
        var temporaryDestinationPath = Environment.ExpandEnvironmentVariables(temporarySubPath);

        try
        {
            Log.Information("Received new zip file: {zipPath}. Start validating file type", zipPath);
            _zipContentValidator.ValidateZipContent(zipPath);

            Log.Information("Creating temporary destination directory");
            ZipFile.ExtractToDirectory(zipPath, temporaryDestinationPath);

            Log.Information("Validating party.XML and extracting application number");
            var applicationNumber = _xmlValidator.ValidateXml();

            Log.Information("Creating final destination directory");
            var finalDirectoryName = applicationNumber + "-" + Guid.NewGuid();
            var finalDestinationPath = Environment.ExpandEnvironmentVariables(partialDestinationPath) + finalDirectoryName;
            Directory.Move(temporaryDestinationPath, finalDestinationPath);
            
            Log.Information("Sending successful email for application: {folderName}", finalDirectoryName);
            await _emailService.SendEmail("Successful");
        }
        catch (InvalidFileTypeException exception)
        {
            var errorMessage = "Invalid file type is included in zip folder";
            Log.Information(exception, errorMessage);
            await _emailService.SendEmail(errorMessage);
        }
        catch (MissingPartyXmlException exception)
        {
            var errorMessage = "party.xml file is missing in zip folder";
            Log.Information(exception, errorMessage);
            await _emailService.SendEmail(errorMessage);
        }
        catch (InvalidDataException exception)
        {
            var errorMessage = "Uploaded zip folder is corrupted";
            Log.Information(exception, errorMessage);
            await _emailService.SendEmail(errorMessage);
        }
        catch (Exception exception)
        {
            var errorMessage = "Failed to extract zip";
            Log.Error(exception, errorMessage);
            await _emailService.SendEmail(errorMessage);
        }
        finally
        {
            if (Directory.Exists(temporaryDestinationPath))
            {
                Directory.Delete(temporaryDestinationPath);
            }
            
            // TODO or find a way to name the directory with correct info in the first place
        }
    }
}