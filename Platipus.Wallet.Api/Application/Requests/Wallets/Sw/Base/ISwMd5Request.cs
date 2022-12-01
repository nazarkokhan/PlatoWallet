namespace Platipus.Wallet.Api.Application.Requests.Wallets.Sw.Base;

public interface ISwMd5Request : ISwBaseRequest
{
    public string Md5 { get; }
}