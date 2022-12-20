namespace Platipus.Wallet.Api.Application.Requests.Wallets.Uis.Base;

using System.Xml.Serialization;

#pragma warning disable CS8618
[XmlRoot("UISSYSTEM")]
[XmlInclude(typeof(UisGetBalanceRequest)), XmlInclude(typeof(UisGetBalanceRequest.UisGetBalanceResponse))]
[XmlInclude(typeof(UisChangeBalanceRequest)), XmlInclude(typeof(UisChangeBalanceRequest.UisChangeBalanceResponse))]
[XmlInclude(typeof(UisStatusRequest)), XmlInclude(typeof(UisStatusRequest.UisStatusResponse))]
[XmlInclude(typeof(UisAuthenticateRequest)), XmlInclude(typeof(UisAuthenticateRequest.UisAuthenticateResponse))]
public class UisResponseContainer
{
    [XmlElement("REQUEST")]
    public object Request { get; set; }

    [XmlElement("TIME")]
    public DateTime Time { get; set; } = DateTime.UtcNow;

    [XmlElement("RESPONSE")]
    public object Response { get; set; }
}