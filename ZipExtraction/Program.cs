using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using ZipExtraction.Configurations;
using ZipExtraction.Services;
using ZipExtraction.Validators;

namespace ZipExtraction;

class Program
{
    static async Task Main(string[] args)
    {
        // Startup startup = new Startup();
        // IServiceCollection services = new ServiceCollection();
        //startup.ConfigureServices(services);

        var container = BuildContainer();

        // TODO actually path in Federal Court of Australia server
        var downloadsPath = Environment.ExpandEnvironmentVariables("%userprofile%/Downloads/");
        var zipFileName = "sample valid zipfile.zip"; // TODO iterate all zip files
        var zipPath = downloadsPath + zipFileName;

        var service = new ZipExtractService(container.Resolve<IZipContentValidator>(),
            container.Resolve<IXmlValidator>(), container.Resolve<IEmailService>());
        await service.ExtractZip(zipPath);

        //TODO after extracting a zip, shall the zip file be moved to somewhere else, or somehow marked as processed??
    }

    private static IContainer BuildContainer()
    {
        var configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddEnvironmentVariables();

        var configuration = configurationBuilder.Build();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        IServiceCollection services = new ServiceCollection();
        services.Configure<ZipValidationConfiguration>(options =>
        {
            options.PartyXsdLocation = configuration.GetValue<string>("PartyXsdLocation");
            options.ValidFileExtensions = configuration.GetValue<string>("ValidFileExtensions").Split(",").ToList();
        });
        services.Configure<ZipExtractionSetting>(configuration);



        var containerBuilder = new ContainerBuilder();

        containerBuilder.Populate(services);
        containerBuilder.RegisterInstance(Log.Logger).AsImplementedInterfaces();
        containerBuilder.RegisterType<XmlValidator>().AsImplementedInterfaces();
        containerBuilder.RegisterType<ZipContentValidator>().AsImplementedInterfaces();
        containerBuilder.RegisterType<EmailService>().AsImplementedInterfaces();

        return containerBuilder.Build();
    }
}
