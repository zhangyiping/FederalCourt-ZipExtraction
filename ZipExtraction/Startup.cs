using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;
using ZipExtraction.Configurations;
using ZipExtraction.Validators;

namespace ZipExtraction;

public class Startup
{
    private ServiceProvider _serviceProvider;
    IConfigurationRoot Configuration { get; }

    public Startup()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddEnvironmentVariables();

        Configuration = builder.Build();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(Configuration)
            .CreateLogger();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.Configure<ZipValidationConfiguration>(options =>
        {
            options.PartyXsdLocation = Configuration.GetValue<string>("PartyXsdLocation");
            options.ValidFileExtensions = Configuration.GetValue<string>("ValidFileExtensions").Split(",").ToList();
        });
        services.Configure<ZipExtractionSetting>(options => Configuration.GetSection("ExtractConfiguration").Bind(options));
    }
}