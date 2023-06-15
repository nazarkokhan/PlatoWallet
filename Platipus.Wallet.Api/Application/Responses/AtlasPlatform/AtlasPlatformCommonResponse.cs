namespace Platipus.Wallet.Api.Application.Responses.AtlasPlatform;

public sealed record AtlasPlatformCommonResponse(
    string Currency, int Balance, string ClientId);