namespace Platipus.Wallet.Api.Application.Requests.Wallets.AtlasPlatform.Base;

public record AtlasPlatformErrorResponse(
    string Error, int ErrorCode);