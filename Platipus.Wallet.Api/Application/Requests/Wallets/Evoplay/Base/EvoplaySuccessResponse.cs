namespace Platipus.Wallet.Api.Application.Requests.Wallets.Evoplay.Base;

public sealed record EvoplaySuccessResponse<TData>(TData Data)
{
    public bool Success { get; init; } = true;
}