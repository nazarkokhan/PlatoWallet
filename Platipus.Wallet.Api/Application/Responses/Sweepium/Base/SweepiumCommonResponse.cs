using System.Text.Json.Serialization;
using Platipus.Wallet.Api.Application.Results.Sweepium;

namespace Platipus.Wallet.Api.Application.Responses.Sweepium.Base;

public record SweepiumCommonResponse(
    bool Result = true,
    [property: JsonPropertyName("err_desc")] string ErrDesc = null!,
    [property: JsonPropertyName("err_code")] int ErrCode = 0);