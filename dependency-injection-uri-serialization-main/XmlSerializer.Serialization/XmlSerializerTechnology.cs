using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using LogerExtensionDelegate;
using Microsoft.Extensions.Logging;
using Serialization;
using UriSerializationHelper;

namespace XmlSerializer.Serialization;

public class XmlSerializerTechnology : IDataSerializer<Uri>
{
    private readonly string path;
    private readonly ILogger<XmlSerializerTechnology>? logger;

    public XmlSerializerTechnology(string? path, ILogger<XmlSerializerTechnology>? logger = default)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("Path cannot be null or empty.", nameof(path));
        }

        this.path = path;
        this.logger = logger;
    }

    public void Serialize(IEnumerable<Uri>? source)
    {
        ArgumentNullException.ThrowIfNull(source);

        try
        {
            var container = new UriContainer(source.Select(u => u.ToSerializableObject()));

            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(UriContainer));

            var settings = new XmlWriterSettings
            {
                Indent = true,
                Encoding = System.Text.Encoding.UTF8,
                OmitXmlDeclaration = false,
            };

            var ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty); // убираем xmlns:xsi / xmlns:xsd

            using var fs = new FileStream(this.path, FileMode.Create, FileAccess.Write, FileShare.Read);
            using var writer = XmlWriter.Create(fs, settings);
            serializer.Serialize(writer, container, ns);
        }
        catch (Exception ex)
        {
            LogerExtension.FastLoggerMessage(this.logger, ex.Message, ex);
            throw;
        }
    }
}
