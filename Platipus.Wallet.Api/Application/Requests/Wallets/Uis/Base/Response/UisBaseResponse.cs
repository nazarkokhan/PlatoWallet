namespace Platipus.Wallet.Api.Application.Requests.Wallets.Uis.Base.Response;

using System.Xml.Serialization;

[XmlRoot("RESPONSE")]
public record UisBaseResponse
{
    [XmlElement("RESULT")]
    public string Result { get; set; } = "OK";
}