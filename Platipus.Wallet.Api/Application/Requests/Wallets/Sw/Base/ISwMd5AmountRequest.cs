namespace Platipus.Wallet.Api.Application.Requests.Wallets.Sw.Base;

public interface ISwMd5AmountRequest : ISwMd5Request
{
    public string Amount { get; }
}