namespace Platipus.Wallet.Api.Application.Requests.Wallets.Softswiss;

using System.Text.Json.Serialization;
using Base;
using Microsoft.Extensions.Options;
using Results.ResultToResultMappers;
using Services.Wallet;
using StartupSettings.Options;

public record SoftswissPlayRequest(
        string SessionId,
        string UserId,
        string Currency,
        string Game,
        string? GameId,
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
            var action = request.Actions?.SingleOrDefault();

            Response response;
            switch (action?.Action)
            {
                case null:
                {
                    var walletResult = await _wallet.GetBalanceAsync(request.SessionId, cancellationToken: cancellationToken);
                    if (walletResult.IsFailure)
                        return walletResult.ToSoftswissResult<Response>();
                    var data = walletResult.Data;

                    response = new Response(
                        _currencyMultipliers.GetSumOut(request.Currency, data.Balance),
                        request.Game,
                        null);

                    break;
                }

                case "bet" or "win" when request.GameId is null:
                    return SoftswissResultFactory.Failure<Response>(SoftswissErrorCode.BadRequest);

                case "bet":
                {
                    var walletResult = await _wallet.BetAsync(
                        request.SessionId,
                        request.GameId,
                        action.ActionId,
                        _currencyMultipliers.GetSumIn(request.Currency, action.Amount),
                        request.Currency,
                        request.Finished ?? false,
                        cancellationToken: cancellationToken);
                    if (walletResult.IsFailure)
                        return walletResult.ToSoftswissResult<Response>();
                    var data = walletResult.Data;

                    response = new Response(
                        _currencyMultipliers.GetSumOut(request.Currency, data.Balance),
                        request.Game,
                        new List<PlayTransaction>
                        {
                            new(action.ActionId, data.Transaction.InternalId, data.Transaction.CreatedDate)
                        });

                    break;
                }

                case "win":
                {
                    var walletResult = await _wallet.WinAsync(
                        request.SessionId,
                        request.GameId,
                        action.ActionId,
                        _currencyMultipliers.GetSumIn(request.Currency, action.Amount),
                        request.Finished ?? false,
                        request.Currency,
                        cancellationToken: cancellationToken);
                    if (walletResult.IsFailure)
                        return walletResult.ToSoftswissResult<Response>();
                    var data = walletResult.Data;

                    response = new Response(
                        _currencyMultipliers.GetSumOut(request.Currency, data.Balance),
                        request.Game,
                        new List<PlayTransaction>
                        {
                            new(action.ActionId, data.Transaction.InternalId, data.Transaction.CreatedDate)
                        });

                    break;
                }

                default:
                    return SoftswissResultFactory.Failure<Response>(SoftswissErrorCode.BadRequest);
            }

            return SoftswissResultFactory.Success(response);
        }
    }

    public record Response(
        long Balance,
        string GameId,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] List<PlayTransaction>? Transactions);

    public record PlayTransaction(
        string ActionId,
        string TxId,
        DateTime ProcessedAt,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] long? BonusAmount = null);

    public record PlayAction(
        string Action,
        long Amount,
        string ActionId,
        double? JackpotContribution,
        long? JackpotWin);
}