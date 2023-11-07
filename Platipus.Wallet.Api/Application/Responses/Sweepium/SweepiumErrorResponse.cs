using System.Text.Json.Serialization;

namespace Platipus.Wallet.Api.Application.Responses.Sweepium.Base;

public record SweepiumErrorResponse(
    [property: JsonPropertyName("err_desc")] string ErrDesc,
    [property: JsonPropertyName("err_code")] int ErrCode,
    bool Result = false) : SweepiumCommonResponse
{
}