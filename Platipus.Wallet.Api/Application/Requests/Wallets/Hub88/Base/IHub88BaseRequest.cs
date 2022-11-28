namespace Platipus.Wallet.Api.Application.Requests.Wallets.Hub88.Base;

using Requests.Base;

public interface IHub88BaseRequest : IBaseWalletRequest
{
    public string SupplierUser { get; }

    public Guid Token { get; }

    public string RequestUuid { get; }
}