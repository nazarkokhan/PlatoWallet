namespace Platipus.Wallet.Api.Application.Requests.Wallets.Sw.Base.Response;

using System.Text.Json.Serialization;

public record SwBetWinRefundFreespinResponse(decimal Balance)
{
    [JsonPropertyName("successCode")]
    public int SuccessCode
        => 0;
}