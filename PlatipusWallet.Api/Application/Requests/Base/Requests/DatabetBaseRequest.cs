namespace PlatipusWallet.Api.Application.Requests.Base.Requests;

public abstract record DatabetBaseRequest(string PlayerId, string Hash) : ISourceRequest
{
    public abstract string GetSource();
}