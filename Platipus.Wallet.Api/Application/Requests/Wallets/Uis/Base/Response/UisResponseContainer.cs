namespace Platipus.Wallet.Api.Application.Requests.Wallets.Uis.Base.Response;

using System.Globalization;
using System.Xml.Serialization;

[XmlRoot("UIS")]
public class UisResponseContainer<TRequest, TResponse>
{
    public UisResponseContainer()
        : this(default!, default!)
    {
    }

    public UisResponseContainer(TRequest request, TResponse response)
    {
        Request = request;
        Time = DateTime.UtcNow.ToString("dd MMM yyyy HH:mm:ss", CultureInfo.InvariantCulture);
        Response = response;
    }

    [XmlElement("REQUEST", Order = 1)]
    public TRequest Request { get; set; }

    [XmlElement("TIME", Order = 2)]
    public string Time { get; set; }

    [XmlElement("RESPONSE", Order = 3)]
    public TResponse Response { get; set; }
}