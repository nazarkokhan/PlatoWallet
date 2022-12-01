namespace Platipus.Wallet.Api.Application.Requests.Wallets.Sw.Base;

public interface ISwHashRequest : ISwBaseRequest
{
    public string Hash { get; }
}