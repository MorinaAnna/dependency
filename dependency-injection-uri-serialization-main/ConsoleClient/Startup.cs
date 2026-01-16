using Conversion;
using ExportDataService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using UriConversion;
using Validation;

namespace ConsoleClient;

public static class Startup
{
    public static IServiceProvider CreateServiceProvider()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

        _ = LogManager.Setup()
            .SetupExtensions(s => s.RegisterConfigSettings(configuration))
            .GetCurrentClassLogger();

        return new ServiceCollection()
            .AddTransient<IValidator<string>, UriValidator>()
            .AddTransient<IConverter<Uri?>, UriConverter>()
            .AddTransient<ExportDataService<Uri>>()
            .UseExportDataServices(configuration, configuration["format"], configuration["mode"])
            .AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                loggingBuilder.AddNLog(configuration);
            })
            .BuildServiceProvider();
    }
}
