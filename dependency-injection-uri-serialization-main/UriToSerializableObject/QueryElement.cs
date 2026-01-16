using System.Xml.Serialization;

namespace UriSerializationHelper;

public class QueryElement
{
    [XmlAttribute("key")]
    public string Key { get; set; } = null!;

    [XmlAttribute("value")]
    public string Value { get; set; } = null!;
}
