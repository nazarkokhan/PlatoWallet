namespace Platipus.Wallet.Api.Application.Requests.Wallets.Sw.Base.Response;

using System.Text.Json.Serialization;
using Results.Sw;

public record SwErrorResponse([property: JsonPropertyName("errorCode")] int ErrorCode)
{
    public SwErrorResponse(SwErrorCode errorCode)
        : this((int)errorCode)
    {
    }
}