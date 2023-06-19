using System.Globalization;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Base;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Models;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Responses;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Results;
using Platipus.Wallet.Api.Application.Results.EmaraPlay;
using Platipus.Wallet.Api.Application.Results.EmaraPlay.WithData;
using Platipus.Wallet.Api.Application.Results.ResultToResultMappers;
using Platipus.Wallet.Api.Application.Services.Wallet;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay;

//TODO fix spacing for request records everywhere for emara play
// public sealed record EmaraPlayBetRequest(
//     string User,
//     string Game,
//     string Bet,
//     string Provider,
//     string Token,
//     string Transaction,
//     string Amount,
//     string? BonusCode,
//     string? BonusAmount,
//     List<Jackpot>? Jackpots, //TODO null assigning is redundant, it is deserialized anyway
//     string? Ip) : IEmaraPlayBaseRequest, IRequest<IEmaraPlayResult<EmaraPlayBetResponse>>
public sealed record EmaraPlayBetRequest(
    string User,
    string Game,
    string Bet,
    string Provider,
    string Token,
    string Transaction,
    decimal Amount,
    string? BonusCode = null,
    string? BonusAmount = null,
    List<Jackpot>? Jackpots = null,
    string? Ip = null) : IEmaraPlayBaseRequest, IRequest<IEmaraPlayResult<EmaraPlayBetResponse>>
{
    public sealed class Handler : IRequestHandler<EmaraPlayBetRequest, IEmaraPlayResult<EmaraPlayBetResponse>>
    {
        private readonly IWalletService _walletService;

        public Handler(IWalletService walletService)
        {
            _walletService = walletService;
        }

        public async Task<IEmaraPlayResult<EmaraPlayBetResponse>> Handle(
            EmaraPlayBetRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _walletService.BetAsync(
                request.Token,
                request.Bet,
                request.Transaction,
                request.Amount, //TODO just accept decimal in contract
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                //TODO write CommonResultToEmaraPlayMappers class because you just return 1 error for all any error from walletResult
                return EmaraPlayResultFactory.Failure<EmaraPlayBetResponse>(EmaraPlayErrorCode.InternalServerError);
            var data = walletResult.Data; //TODO just use variable to shorten code

            var betResult = new BetResult(
                data.Currency,
                data.Balance.ToString(CultureInfo.InvariantCulture),
                data.Transaction.InternalId,
                data.Transaction.Id);

            var response = new EmaraPlayBetResponse(
                ((int)EmaraPlayErrorCode.Success).ToString(),
                EmaraPlayErrorCode.Success.ToString(),
                betResult);

            return EmaraPlayResultFactory.Success(response);
        }
    }
}