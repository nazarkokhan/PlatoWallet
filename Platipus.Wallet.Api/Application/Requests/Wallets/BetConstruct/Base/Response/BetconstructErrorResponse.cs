namespace Platipus.Wallet.Api.Application.Requests.Wallets.BetConstruct.Base.Response;

using System.Text.Json.Serialization;
using Humanizer;
using Results.BetConstruct;

public record BetconstructErrorResponse(
    [property: JsonPropertyName("err_desc")] string ErrDesc,
    [property: JsonPropertyName("err_code")] int ErrCode) : BetconstructBaseResponse(false)
{
    public BetconstructErrorResponse(BetconstructErrorCode error)
        : this(error.Humanize(), (int)error)
    {
    }
}