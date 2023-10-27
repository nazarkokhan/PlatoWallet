using Platipus.Wallet.Api.Application.Results.Sweepium;

namespace Platipus.Wallet.Api.Application.Responses.Sweepium.Base;

public sealed record SweepiumErrorResponse(
    SweepiumErrorCode StatusCode, 
    string StatusMessage) : SweepiumCommonResponse(
    StatusCode);