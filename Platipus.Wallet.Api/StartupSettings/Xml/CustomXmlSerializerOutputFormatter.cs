namespace Platipus.Wallet.Api.StartupSettings.Xml;

using System.Xml;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc.Formatters;

public class CustomXmlSerializerOutputFormatter : XmlSerializerOutputFormatter
{
    protected override void Serialize(XmlSerializer xmlSerializer, XmlWriter xmlWriter, object? value)
    {
        var emptyNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
        xmlSerializer.Serialize(xmlWriter, value, emptyNamespaces);
    }

    public override XmlWriter CreateXmlWriter(TextWriter writer, XmlWriterSettings settings)
    {
        return new CustomXmlWriter(writer);
    }
}