namespace Platipus.Wallet.Api.Application.Requests.Wallets.Dafabet.Base;

using Requests.Base;

public abstract record DatabetBaseRequest(string PlayerId, string Hash) : BaseRequest, ISourceRequest
{
    public abstract string GetSource();
}