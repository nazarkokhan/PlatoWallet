using System.Text.Json.Serialization;
using Platipus.Wallet.Api.Application.Results.Sweepium;

namespace Platipus.Wallet.Api.Application.Responses.Sweepium.Base;

public sealed record SweepiumErrorResponse(bool Result,
    [property: JsonPropertyName("err_desc")] string ErrorDescription,
    [property: JsonPropertyName("err_code")] int ErrorCode) : SweepiumCommonResponse(
    Result);