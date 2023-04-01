namespace Platipus.Wallet.Api.Application.Requests.Wallets.Uis.Base.Response;

using System.Xml.Serialization;
using Results.Uis;

[XmlRoot("RESPONSE")]
public class UisErrorResponse
{
    public UisErrorResponse()
        : this(UisErrorCode.InternalError)
    {
    }

    public UisErrorResponse(UisErrorCode code)
    {
        Code = (int)code;
    }

    [XmlElement("CODE")]
    public int Code { get; init; }

    [XmlElement("RESULT")]
    public string Result { get; init; } = "FAILED";
}