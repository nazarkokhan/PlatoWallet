namespace Platipus.Wallet.Api.Application.Requests.Wallets.Softswiss;

using Base;
using Extensions;
using Microsoft.Extensions.Options;
using Results.ResultToResultMappers;
using Services.Wallet;
using Services.Wallet.DTOs;
using StartupSettings.Options;

public record SoftswissPlayRequest(
        Guid SessionId,
        string UserId,
        string Currency,
        string Game,
        string GameId,
        bool? Finished,
        List<SoftswissPlayRequest.PlayAction>? Actions)
    : ISoftswissBaseRequest, IRequest<ISoftswissResult<SoftswissPlayRequest.Response>>
{
    public class Handler : IRequestHandler<SoftswissPlayRequest, ISoftswissResult<Response>>
    {
        private readonly IWalletService _wallet;
        private readonly SoftswissCurrenciesOptions _currencyMultipliers;

        public Handler(IWalletService wallet, IOptions<SoftswissCurrenciesOptions> currencyMultipliers)
        {
            _wallet = wallet;
            _currencyMultipliers = currencyMultipliers.Value;
        }

        public async Task<ISoftswissResult<Response>> Handle(
            SoftswissPlayRequest request,
            CancellationToken cancellationToken)
        {
            var action = request.Actions?.FirstOrDefault();

            Response response;
            switch (action?.Action)
            {
                case null:
                    var balanceRequest = request.Map(r => new GetBalanceRequest(r.SessionId, r.UserId));

                    var balanceResult = await _wallet.GetBalanceAsync(balanceRequest, cancellationToken);
                    if (balanceResult.IsFailure)
                        return balanceResult.ToSoftswissResult<Response>();

                    response = balanceResult.Data.Map(
                        d => new Response(
                            _currencyMultipliers.GetSumOut(request.Currency, d.Balance),
                            request.GameId,
                            null));
                    break;

                case "bet":
                    var betRequest = request.Map(
                        r => new BetRequest(
                            r.SessionId,
                            r.UserId,
                            r.Currency,
                            r.GameId,
                            action.ActionId,
                            r.Finished ?? false,
                            _currencyMultipliers.GetSumIn(request.Currency, action.Amount)));

                    var betResult = await _wallet.BetAsync(betRequest, cancellationToken);
                    if (betResult.IsFailure)
                        return betResult.ToSoftswissResult<Response>();

                    response = betResult.Data.Map(
                        d => new Response(
                            _currencyMultipliers.GetSumOut(request.Currency, d.Balance),
                            request.GameId,
                            new List<PlayTransaction> { new(action.ActionId, d.InternalTransactionId, d.CreatedDate) }));
                    break;

                case "win":
                    var winRequest = request.Map(
                        r => new WinRequest(
                            r.SessionId,
                            r.UserId,
                            r.Currency,
                            r.Game,
                            r.GameId,
                            action.ActionId,
                            r.Finished ?? true,
                            _currencyMultipliers.GetSumIn(request.Currency, action.Amount)));

                    var winResult = await _wallet.WinAsync(winRequest, cancellationToken);
                    if (winResult.IsFailure)
                        return winResult.ToSoftswissResult<Response>();

                    response = winResult.Data.Map(
                        d => new Response(
                            _currencyMultipliers.GetSumOut(request.Currency, d.Balance),
                            request.GameId,
                            new List<PlayTransaction> { new(action.ActionId, d.InternalTransactionId, d.CreatedDate) }));
                    break;

                default:
                    return SoftswissResultFactory.Failure<Response>(SoftswissErrorCode.BadRequest);
            }

            return SoftswissResultFactory.Success(response);
        }
    }

    public record Response(
        long Balance,
        string GameId,
        List<PlayTransaction>? Transactions);

    public record PlayTransaction(
        string ActionId,
        string TxId,
        DateTime ProcessedAt,
        long? BonusAmount = null);

    public record PlayAction(
        string Action,
        long Amount,
        string ActionId,
        double? JackpotContribution,
        long? JackpotWin);
}