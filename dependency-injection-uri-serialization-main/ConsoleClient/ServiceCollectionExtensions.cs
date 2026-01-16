using DataReceiving;
using InMemoryReceiver;
using JsonSerializer.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serialization;
using TextFileReceiver;
using XDomWriter.Serialization;
using XmlSerializer.Serialization;

namespace ConsoleClient;

/// <summary>
/// Extension methods for service collection.
/// </summary>
internal static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add northwind services to service collection.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <param name="configuration">Configuration.</param>
    /// <param name="format">format provider.</param>
    /// <param name="mode">output format mode.</param>
    /// <returns>IServiceCollection.</returns>
    public static IServiceCollection UseExportDataServices(this IServiceCollection services, IConfiguration configuration, string format, string mode)
    {
        string path = Directory.GetCurrentDirectory();

        string txtPath = Path.Combine(path, configuration["textFilePath"]) ?? throw new ArgumentNullException("textFilePath");
        string xmlPath = Path.Combine(path, configuration["xmlFilePath"]) ?? throw new ArgumentNullException("xmlFilePath");
        string jsonPath = Path.Combine(path, configuration["jsonFilePath"]) ?? throw new ArgumentNullException("jsonFilePath");

        return format switch
        {
            "xml" when mode == "inFile" => services
                .AddTransient<IDataReceiver>(service => new TextStreamReceiver(txtPath, service.GetRequiredService<ILogger<TextStreamReceiver>>()))
                .AddTransient<IDataSerializer<Uri>, XDomTechnology>(service => new XDomTechnology(xmlPath, service.GetRequiredService<ILogger<XDomTechnology>>()))
                .AddTransient<IDataSerializer<Uri>, XmlSerializerTechnology>(service => new XmlSerializerTechnology(xmlPath, service.GetRequiredService<ILogger<XmlSerializerTechnology>>())),
            "xml" when mode == "inMemory" => services
                .AddTransient<IDataReceiver>(service => new InMemoryDataReceiver())
                .AddTransient<IDataSerializer<Uri>, XDomTechnology>(service => new XDomTechnology(xmlPath, service.GetRequiredService<ILogger<XDomTechnology>>())),
            "json" when mode == "inFile" => services
                .AddTransient<IDataReceiver>(service => new TextStreamReceiver(txtPath, service.GetRequiredService<ILogger<TextStreamReceiver>>()))
                .AddTransient<IDataSerializer<Uri>, JsonSerializerTechnology>(service => new JsonSerializerTechnology(jsonPath)),
            "json" when mode == "inMemory" => services
                .AddTransient<IDataReceiver>(service => new InMemoryDataReceiver())
                .AddTransient<IDataSerializer<Uri>, JsonSerializerTechnology>(service => new JsonSerializerTechnology(jsonPath)),
            _ => throw new ArgumentException(nameof(format), format, null)
        };
    }
}
