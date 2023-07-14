namespace Platipus.Wallet.Api.Application.Requests.Wallets.Uranus.Base;

public interface IUranusBoxResponse<out TData>
{
    public bool Success { get; }

    public TData Data { get; }

    public UranusCommonErrorResponse Error { get; }
}