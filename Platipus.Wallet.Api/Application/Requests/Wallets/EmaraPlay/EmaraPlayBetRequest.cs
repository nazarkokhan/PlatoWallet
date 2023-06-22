namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay;

using System.Text.Json.Serialization;
using Application.Results.ResultToResultMappers;
using Base;
using Models;
using Responses;
using Results;
using Platipus.Wallet.Api.Application.Results.EmaraPlay;
using Platipus.Wallet.Api.Application.Results.EmaraPlay.WithData;
using Platipus.Wallet.Api.Application.Services.Wallet;

public sealed record EmaraPlayBetRequest(
    string User,
    string Game,
    string Bet,
    string Provider,
    string Token,
    string Transaction,
    decimal Amount,
    [property: JsonPropertyName("bonusCode")]string? BonusCode = null,
    [property: JsonPropertyName("bonusAmount")]string? BonusAmount = null,
    List<Jackpot?>? Jackpots = null,
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
                transactionId: request.Transaction,
                request.Amount,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToEmaraPlayResult<EmaraPlayBetResponse>();
            
            var data = walletResult.Data;
            var response = new EmaraPlayBetResponse(
                new BetResult(data.Currency, data.Balance, data.Transaction.InternalId, data.Transaction.Id));

            return EmaraPlayResultFactory.Success(response);
        }
    }
}