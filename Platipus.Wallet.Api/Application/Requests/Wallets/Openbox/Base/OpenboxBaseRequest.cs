namespace Platipus.Wallet.Api.Application.Requests.Wallets.Openbox.Base;

using System.Text.Json.Serialization;
using Requests.Base;

public abstract record OpenboxBaseRequest(OpenboxSingleRequest Request) : BaseRequest
{
    public OpenboxBaseRequest()
        : this((OpenboxSingleRequest)default!)
    {
    }

    [JsonIgnore]
    public OpenboxSingleRequest Request { get; set; } = Request;
}