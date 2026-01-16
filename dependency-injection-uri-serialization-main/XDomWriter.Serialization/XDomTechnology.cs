using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using LogerExtensionDelegate;
using Microsoft.Extensions.Logging;
using Serialization;
using UriSerializationHelper;

namespace XDomWriter.Serialization;

public class XDomTechnology : IDataSerializer<Uri>
{
    private readonly string path;
    private readonly ILogger<XDomTechnology>? logger;

    public XDomTechnology(string? path, ILogger<XDomTechnology>? logger = default)
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
            var items = source.Select(u => u.ToSerializableObject()).ToList();

            var doc = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement(
                    "uriAdresses",
                    items.Select(item =>
                    {
                        var uriEl = new XElement(
                            "uriAdress",
                            new XElement("scheme", new XAttribute("name", item.Scheme)),
                            new XElement("host", new XAttribute("name", item.Host)),
                            new XElement(
                                "path",
                                item.Path
                                    .Select(seg => new XElement("segment", seg))));

                        var query = item.QuerySerializable;
                        if (query is not null && query.Count > 0)
                        {
                            uriEl.Add(
                                new XElement(
                                    "query",
                                    query.Select(p =>
                                        new XElement(
                                            "parameter",
                                            new XAttribute("key", p.Key),
                                            new XAttribute("value", p.Value)))));
                        }

                        return uriEl;
                    })));

            var settings = new XmlWriterSettings
            {
                Indent = true,
                Encoding = System.Text.Encoding.UTF8,
                OmitXmlDeclaration = false,
            };

            using var fs = new FileStream(this.path, FileMode.Create, FileAccess.Write, FileShare.Read);
            using var writer = XmlWriter.Create(fs, settings);
            doc.Save(writer);
        }
        catch (Exception ex)
        {
            LogerExtension.FastLoggerMessage(this.logger, ex.Message, ex);
            throw;
        }
    }
}
