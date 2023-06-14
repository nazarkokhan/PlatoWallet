using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Base;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Results;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Responses;

public sealed record EmaraPlayGetRoundDetailsResponse(
        int Error, string Description, RoundDetailsResult Result) 
    : EmaraPlayErrorResponse(Error, Description);