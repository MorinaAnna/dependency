using System;
using System.IO;
using System.Linq;
using System.Xml;
using LogerExtensionDelegate;
using Microsoft.Extensions.Logging;
using Serialization;
using UriSerializationHelper;

namespace XmlDomWriter.Serialization;

public class XmlDomTechnology : IDataSerializer<Uri>
{
    private readonly string path;
    private readonly ILogger<XmlDomTechnology>? logger;

    public XmlDomTechnology(string? path, ILogger<XmlDomTechnology>? logger = default)
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

            var doc = new XmlDocument();

            var decl = doc.CreateXmlDeclaration("1.0", "utf-8", null);
            doc.AppendChild(decl);

            var root = doc.CreateElement("uriAdresses");
            doc.AppendChild(root);

            foreach (var item in items)
            {
                var uriNode = doc.CreateElement("uriAdress");
                root.AppendChild(uriNode);

                var scheme = doc.CreateElement("scheme");
                var schemeAttr = doc.CreateAttribute("name");
                schemeAttr.Value = item.Scheme;
                scheme.Attributes.Append(schemeAttr);
                uriNode.AppendChild(scheme);

                var host = doc.CreateElement("host");
                var hostAttr = doc.CreateAttribute("name");
                hostAttr.Value = item.Host;
                host.Attributes.Append(hostAttr);
                uriNode.AppendChild(host);

                var pathNode = doc.CreateElement("path");
                uriNode.AppendChild(pathNode);

                foreach (var seg in item.Path ?? Enumerable.Empty<string>())
                {
                    var segNode = doc.CreateElement("segment");
                    segNode.InnerText = seg;
                    pathNode.AppendChild(segNode);
                }

                var query = item.QuerySerializable;
                if (query is not null && query.Count > 0)
                {
                    var queryNode = doc.CreateElement("query");
                    uriNode.AppendChild(queryNode);

                    foreach (var p in query)
                    {
                        var paramNode = doc.CreateElement("parameter");

                        var keyAttr = doc.CreateAttribute("key");
                        keyAttr.Value = p.Key;
                        paramNode.Attributes.Append(keyAttr);

                        var valAttr = doc.CreateAttribute("value");
                        valAttr.Value = p.Value;
                        paramNode.Attributes.Append(valAttr);

                        queryNode.AppendChild(paramNode);
                    }
                }
            }

            // Сохраняем с форматированием
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
