using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Base;
using Platipus.Wallet.Api.Application.Results.EmaraPlay;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Responses;

public sealed record EmaraPlayCancelResponse : EmaraPlayErrorResponse
{
    public EmaraPlayCancelResponse(EmaraPlayErrorCode errorCode) : base(errorCode)
    {
    }
}