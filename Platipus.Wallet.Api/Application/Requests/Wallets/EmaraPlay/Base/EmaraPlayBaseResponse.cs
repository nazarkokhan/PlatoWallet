namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Base;

using Application.Results.EmaraPlay;
using Humanizer;

public abstract record EmaraPlayBaseResponse
{
    protected EmaraPlayBaseResponse(EmaraPlayErrorCode errorCode)
    {
        Error = (int)errorCode;
        Description = errorCode.Humanize();
    }

    public int Error { get; private init; }
    public string Description { get; private init; }
}
    