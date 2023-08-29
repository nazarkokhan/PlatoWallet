namespace Platipus.Wallet.Api.Application.Services.EverymatrixGameApi.Responses;

using JetBrains.Annotations;

[PublicAPI]
public record EverymatrixErrorGameApiResponse(
    int Error,
    string Message,
    bool Success,
    string Description);