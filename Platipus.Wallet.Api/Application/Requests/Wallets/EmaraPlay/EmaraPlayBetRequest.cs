using System.Globalization;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Base;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Models;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Responses;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Results;
using Platipus.Wallet.Api.Application.Results.EmaraPlay;
using Platipus.Wallet.Api.Application.Results.EmaraPlay.WithData;
using Platipus.Wallet.Api.Application.Services.Wallet;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay;

public sealed record EmaraPlayBetRequest(
    string User, string Game, string Bet, 
    string Provider, string Token, string Transaction,
    string Amount, string? BonusCode = null, string? BonusAmount = null, 
    List<Jackpot>? Jackpots = null, 
    string? Ip = null) : IEmaraPlayBaseRequest, IRequest<IEmaraPlayResult<EmaraPlayBetResponse>>
{
    public sealed class Handler :
        IRequestHandler<EmaraPlayBetRequest, IEmaraPlayResult<EmaraPlayBetResponse>>
    {
        private readonly IWalletService _walletService;

        public Handler(IWalletService walletService)
        {
            _walletService = walletService;
        }

        public async Task<IEmaraPlayResult<EmaraPlayBetResponse>> Handle(
            EmaraPlayBetRequest request, CancellationToken cancellationToken)
        {
            var walletResult = await _walletService.BetAsync(
                request.Token,
                request.Bet,
                request.Transaction,
                amount: Convert.ToDecimal(request.Amount),
                cancellationToken: cancellationToken);
            
            if (walletResult.IsFailure)
                return EmaraPlayResultFactory.Failure<EmaraPlayBetResponse>(EmaraPlayErrorCode.InternalServerError);

            var betResult = new BetResult(
                walletResult.Data.Currency, walletResult.Data.Balance.ToString(CultureInfo.InvariantCulture), 
                walletResult.Data.Transaction.InternalId, walletResult.Data.Transaction.Id);
            var response = new EmaraPlayBetResponse(((int)EmaraPlayErrorCode.Success).ToString(), 
                EmaraPlayErrorCode.Success.ToString(), betResult);

            return EmaraPlayResultFactory.Success(response);
        }
    }
}