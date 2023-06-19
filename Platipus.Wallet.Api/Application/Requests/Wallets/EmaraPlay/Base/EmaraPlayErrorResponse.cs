using Platipus.Wallet.Api.Application.Results.EmaraPlay;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Base;

public record EmaraPlayErrorResponse : EmaraPlayBaseResponse
{
    public EmaraPlayErrorResponse(EmaraPlayErrorCode errorCode)
        : base(errorCode)
    {
    }
}