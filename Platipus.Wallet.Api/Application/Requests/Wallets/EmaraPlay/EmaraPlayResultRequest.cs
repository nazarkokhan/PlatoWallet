using System.Globalization;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Base;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Models;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Responses;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Results;
using Platipus.Wallet.Api.Application.Results.EmaraPlay;
using Platipus.Wallet.Api.Application.Results.EmaraPlay.WithData;
using Platipus.Wallet.Api.Application.Services.Wallet;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay;

using Application.Results.ResultToResultMappers;

public sealed record EmaraPlayResultRequest(
        string User, string Game, string Bet, 
        decimal Amount, string Transaction, string Provider, 
        string Token, string CloseRound, string BetBonusAmount, 
        List<Jackpot>? Jackpots = null, string? BonusCode = null, 
        string? BonusAmount = null, List<Detail>? Details = null, string? Spins = null) 
    : IEmaraPlayBaseRequest, IRequest<IEmaraPlayResult<EmaraPlayResultResponse>>
{
    public sealed class Handler 
        : IRequestHandler<EmaraPlayResultRequest, IEmaraPlayResult<EmaraPlayResultResponse>>
    {
        private readonly IWalletService _walletService;

        public Handler(IWalletService walletService)
        {
            _walletService = walletService;
        }

        public async Task<IEmaraPlayResult<EmaraPlayResultResponse>> Handle(
            EmaraPlayResultRequest request, CancellationToken cancellationToken)
        {
            var walletResult = await _walletService.WinAsync(
                request.Token,
                request.Bet,
                request.Transaction,
                request.Amount, cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToEmaraPlayResult<EmaraPlayResultResponse>();

            var walletData = walletResult.Data;
            var winResult = new EmaraplayWinResult(walletData.Currency,
                walletData.Balance,
                walletData.Transaction.Id, walletData.Transaction.InternalId);

            var response = new EmaraPlayResultResponse(winResult);
            return EmaraPlayResultFactory.Success(response);

        }
    }
}