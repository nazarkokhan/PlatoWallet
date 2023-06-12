using System.Globalization;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Base;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Models;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Responses;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Results;
using Platipus.Wallet.Api.Application.Results.EmaraPlay;
using Platipus.Wallet.Api.Application.Results.EmaraPlay.WithData;
using Platipus.Wallet.Api.Application.Services.Wallet;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay;

public sealed record EmaraPlayResultRequest(
        string User, string Game, string Bet, 
        string Amount, string Transaction, string Provider, 
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
                decimal.Parse(request.Amount), cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return EmaraPlayResultFactory.Failure<EmaraPlayResultResponse>(EmaraPlayErrorCode.BadParameters);

            var response = new EmaraPlayResultResponse(((int)EmaraPlayErrorCode.Success).ToString(),
                EmaraPlayErrorCode.Success.ToString(), 
                new WinResult(walletResult.Data.Currency, walletResult.Data.Balance.ToString(CultureInfo.InvariantCulture), 
                    walletResult.Data.Transaction.Id, walletResult.Data.Transaction.InternalId));

            return EmaraPlayResultFactory.Success(response);

        }
    }
}