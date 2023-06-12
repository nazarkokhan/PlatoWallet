using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Base;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Models;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Responses;
using Platipus.Wallet.Api.Application.Results.EmaraPlay;
using Platipus.Wallet.Api.Application.Results.EmaraPlay.WithData;
using Platipus.Wallet.Domain.Entities;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay;

public sealed record EmaraPlayResultRequest(
        string User, string Game, string Bet, 
        string Amount, string Transaction, string Provider, 
        string Token, string CloseRound, string BetBonusAmount, 
        List<Jackpot>? Jackpots = null, string? BonusCode = null, 
        string? BonusAmount = null, List<Detail>? Details = null, string? Spins = null) 
    : IEmaraPlayBaseRequest, IRequest<IEmaraPlayResult<EmaraPlayResultResponse>>
{
    public sealed class Handler : IRequestHandler<EmaraPlayResultRequest, IEmaraPlayResult<EmaraPlayResultResponse>>
    {
        public async Task<IEmaraPlayResult<EmaraPlayResultResponse>> Handle(EmaraPlayResultRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}