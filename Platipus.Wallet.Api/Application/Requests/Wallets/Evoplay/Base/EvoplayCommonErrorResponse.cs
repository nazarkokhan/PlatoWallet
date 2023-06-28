namespace Platipus.Wallet.Api.Application.Requests.Wallets.Evoplay.Base;

public sealed record EvoplayCommonErrorResponse(
    string Message, string Code, object Context);