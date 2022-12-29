namespace Platipus.Wallet.Api.Application.Requests.Wallets.Everymatrix.Base;

using Requests.Base;

public interface IEverymatrixBaseRequest : IBaseWalletRequest
{
}

public interface IEverymatrixRequest : IEverymatrixBaseRequest
{
    // public string SupplierUser { get; }
    //
    public Guid Token { get; }
    //
    // public string RequestUuid { get; }
}