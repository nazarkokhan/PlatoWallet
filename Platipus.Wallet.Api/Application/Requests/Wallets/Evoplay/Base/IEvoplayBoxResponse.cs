namespace Platipus.Wallet.Api.Application.Requests.Wallets.Evoplay.Base;

public interface IEvoplayBoxResponse<out TData>
{
    public bool Success { get; }

    public TData Data { get; }

    public EvoplayCommonErrorResponse Error { get; }
}