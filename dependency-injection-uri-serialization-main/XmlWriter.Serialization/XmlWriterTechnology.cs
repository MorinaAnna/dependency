using System;
using System.IO;
using System.Linq;
using System.Security;
using System.Xml;
using LogerExtensionDelegate;
using Microsoft.Extensions.Logging;
using Serialization;
using UriSerializationHelper;

namespace XmlWriter.Serialization;

public class XmlWriterTechnology : IDataSerializer<Uri>
{
    private readonly string path;
    private readonly ILogger<XmlWriterTechnology>? logger;

    public XmlWriterTechnology(string? path, ILogger<XmlWriterTechnology>? logger = default)
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

            var settings = new System.Xml.XmlWriterSettings
            {
                Indent = true,
                Encoding = System.Text.Encoding.UTF8,
                OmitXmlDeclaration = false,
            };

            using var fs = new FileStream(this.path, FileMode.Create, FileAccess.Write, FileShare.Read);
            using var writer = System.Xml.XmlWriter.Create(fs, settings);

            writer.WriteStartDocument();
            writer.WriteStartElement("uriAdresses");

            foreach (var item in items)
            {
                writer.WriteStartElement("uriAdress");

                writer.WriteStartElement("scheme");
                writer.WriteAttributeString("name", item.Scheme);
                writer.WriteEndElement();

                writer.WriteStartElement("host");
                writer.WriteAttributeString("name", item.Host);
                writer.WriteEndElement();

                writer.WriteStartElement("path");
                foreach (var seg in item.Path ?? Enumerable.Empty<string>())
                {
                    writer.WriteElementString("segment", seg);
                }

                writer.WriteEndElement();

                var query = item.QuerySerializable;
                if (query is not null && query.Count > 0)
                {
                    writer.WriteStartElement("query");
                    foreach (var p in query)
                    {
                        writer.WriteStartElement("parameter");
                        writer.WriteAttributeString("key", p.Key);
                        writer.WriteAttributeString("value", p.Value);
                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                }

                writer.WriteEndElement(); // uriAdress
            }

            writer.WriteEndElement(); // uriAdresses
            writer.WriteEndDocument();
        }
        catch (UnauthorizedAccessException ex)
        {
            LogerExtension.FastLoggerMessage(this.logger, ex.Message, ex);
            throw;
        }
        catch (DirectoryNotFoundException ex)
        {
            LogerExtension.FastLoggerMessage(this.logger, ex.Message, ex);
            throw;
        }
        catch (FileNotFoundException ex)
        {
            LogerExtension.FastLoggerMessage(this.logger, ex.Message, ex);
            throw;
        }
        catch (IOException ex)
        {
            LogerExtension.FastLoggerMessage(this.logger, ex.Message, ex);
            throw;
        }
        catch (SecurityException ex)
        {
            LogerExtension.FastLoggerMessage(this.logger, ex.Message, ex);
            throw;
        }
        catch (XmlException ex)
        {
            LogerExtension.FastLoggerMessage(this.logger, ex.Message, ex);
            throw;
        }
    }
}
