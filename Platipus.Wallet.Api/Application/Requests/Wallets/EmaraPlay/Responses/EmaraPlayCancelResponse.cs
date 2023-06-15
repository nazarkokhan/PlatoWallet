using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Base;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Responses;

public sealed record EmaraPlayCancelResponse(int Error, string Description) :
    EmaraPlayErrorResponse(Error, Description);