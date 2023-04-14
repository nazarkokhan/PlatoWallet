namespace Platipus.Wallet.Api.StartupSettings.Xml;

using System.Xml;

public class CustomXmlWriter : XmlTextWriter
{
    public CustomXmlWriter(TextWriter writer)
        : base(writer)
    {
    }

    public override void WriteWhitespace(string? ws)
    {
        WriteString(ws);
    }

    public override void WriteString(string? text)
    {
        base.WriteString(text ?? "");
    }

    public override void WriteStartDocument()
    {
    }

    public override void WriteEndElement()
    {
        WriteFullEndElement();
    }
}