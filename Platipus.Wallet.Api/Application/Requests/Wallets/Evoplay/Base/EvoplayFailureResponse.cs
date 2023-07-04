namespace Platipus.Wallet.Api.Application.Requests.Wallets.Evoplay.Base;

public sealed record EvoplayFailureResponse(
    EvoplayCommonErrorResponse Error)
{
    public bool Success { get; init; } = false;
}