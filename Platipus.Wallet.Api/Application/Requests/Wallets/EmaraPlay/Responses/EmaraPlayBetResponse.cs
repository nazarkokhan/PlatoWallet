using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Results;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Responses;

using Base;

public sealed record EmaraPlayBetResponse(BetResult Result) : EmaraPlayCommonBoxResponse<BetResult>(Result);
