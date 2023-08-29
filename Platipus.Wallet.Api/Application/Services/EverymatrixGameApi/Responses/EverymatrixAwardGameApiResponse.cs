namespace Platipus.Wallet.Api.Application.Services.EverymatrixGameApi.Responses;

public record EverymatrixAwardGameApiResponse(
    string VendorBonusId,
    int Error,
    string Message,
    bool Success);