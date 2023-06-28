namespace Platipus.Wallet.Api.Application.Requests.Wallets.Atlas.Base;

public sealed record AtlasErrorResponse(
    string Error, int ErrorCode);