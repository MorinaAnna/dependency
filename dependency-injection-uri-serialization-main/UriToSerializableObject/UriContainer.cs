using System.Xml.Serialization;

namespace UriSerializationHelper;

[XmlRoot(ElementName = "uriAdresses")]
public class UriContainer
{
    public UriContainer()
    {
        this.UriAddresses = [];
    }

    public UriContainer(IEnumerable<UriAddress> source)
    {
        this.UriAddresses = source.ToArray();
    }

    [XmlElement("uriAdress")]
    public UriAddress[] UriAddresses { get; set; }
}
