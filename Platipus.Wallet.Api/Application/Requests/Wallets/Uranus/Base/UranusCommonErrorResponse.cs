namespace Platipus.Wallet.Api.Application.Requests.Wallets.Uranus.Base;

public sealed record UranusCommonErrorResponse(
    string Message, string Code, object Context);