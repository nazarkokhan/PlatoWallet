namespace Platipus.Wallet.Api.Application.Requests.Wallets.Openbox.Base;

using Requests.Base;

public abstract record OpenboxBaseRequest(Guid Token, OpenboxSingleRequest Request) : BaseRequest
{
    // public OpenboxBaseRequest()
    //     : this((OpenboxSingleRequest)default!)
    // {
    // }
    //
    // [JsonIgnore]
    public OpenboxSingleRequest Request { get; set; } = Request;
}