namespace Platipus.Wallet.Api.Application.Responses.AtlasPlatform;

public sealed record AtlasPlatformGetClientBalanceResponse(
    string Currency, int Balance, string ClientId);