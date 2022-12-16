using Autofac;
using ZipExtraction.Services;
using ZipExtraction.Validators;

namespace ZipExtraction;

class Program
{
    static async Task Main(string[] args)
    {
        var container = BuildContainer();

        // TODO actually path in Federal Court server
        var downloadsPath = Environment.ExpandEnvironmentVariables("%userprofile%/Downloads/");
        var zipFileName = "sample valid zipfile.zip";
        var zipPath = downloadsPath + zipFileName;
        
        var service = new ZipExtractService(container.Resolve<IZipContentValidator>(),
            container.Resolve<IXmlValidator>(), container.Resolve<IEmailService>());
        await service.ExtractZip(zipPath);
        
        //TODO after extracting a zip, shall the zip file be moved to somewhere else, or somehow marked as processed??
    }

    private static IContainer BuildContainer()
    {
        var builder = new ContainerBuilder();

        builder.RegisterType<XmlValidator>().AsImplementedInterfaces();
        builder.RegisterType<ZipContentValidator>().AsImplementedInterfaces();
        builder.RegisterType<EmailService>().AsImplementedInterfaces();
        
        return builder.Build();
    }
    
    //TODO register environment variables in Startup
    //TODO register Serilog or equivalent
    /*
    services.Configure<ZipValidationConfiguration>(options =>
    {
        options.PartyXsdLocation = Configuration.GetValue<string>("PartyXsdLocation");
        options.ValidFileExtensions = Configuration.GetValue<string>("ValidFileExtensions").Split(",").ToList();
    });
    */
}
