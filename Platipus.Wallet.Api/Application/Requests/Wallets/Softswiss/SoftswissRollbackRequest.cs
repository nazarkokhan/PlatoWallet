namespace Platipus.Wallet.Api.Application.Requests.Wallets.Softswiss;

using Base;
using Domain.Entities;
using Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Results.Hub88.WithData.Mappers;
using Services.Wallet;
using Services.Wallet.DTOs;
using StartupSettings.Options;

public record SoftswissRollbackRequest(
        Guid SessionId,
        string UserId,
        string Currency,
        string Game,
        string GameId,
        bool? Finished,
        List<SoftswissRollbackRequest.RollbackAction>? Actions)
    : ISoftswissBaseRequest, IRequest<ISoftswissResult<SoftswissRollbackRequest.Response>>
{
    public class Handler : IRequestHandler<SoftswissRollbackRequest, ISoftswissResult<Response>>
    {
        private readonly IWalletService _wallet;
        private readonly SoftswissCurrenciesOptions _currencyMultipliers;
        private readonly WalletDbContext _context;

        public Handler(IWalletService wallet, IOptions<SoftswissCurrenciesOptions> currencyMultipliers, WalletDbContext context)
        {
            _wallet = wallet;
            _context = context;
            _currencyMultipliers = currencyMultipliers.Value;
        }

        public async Task<ISoftswissResult<Response>> Handle(
            SoftswissRollbackRequest request,
            CancellationToken cancellationToken)
        {
            var action = request.Actions?.FirstOrDefault();

            if (action is not {Action: "rollback"})
                return SoftswissResultFactory.Failure<Response>(SoftswissErrorCode.BadRequest);

            var rollbackRequest = request.Map(
                r => new RollbackRequest(
                    r.SessionId,
                    r.UserId,
                    r.Game,
                    r.GameId,
                    action.OriginalActionId));

            var rollbackResult = await _wallet.RollbackAsync(rollbackRequest, cancellationToken);
            if (rollbackResult.IsFailure)
            {
                var user = await _context.Set<User>()
                    .Where(u => u.Sessions.Any(s => s.Id == request.SessionId))
                    .Select(u => new {u.Balance})
                    .FirstOrDefaultAsync(cancellationToken);

                var balance = user?.Map(u => _currencyMultipliers.GetSumOut(request.Currency, u.Balance));

                return rollbackResult.ToSoftswissResult<Response>(balance);
            }

            var response = rollbackResult.Data.Map(
                d => new Response(
                    _currencyMultipliers.GetSumOut(request.Currency, d.Balance),
                    request.GameId,
                    new List<RollbackTransaction> {new(action.ActionId, d.InternalTransactionId, d.CreatedDate)}));

            return SoftswissResultFactory.Success(response);
        }
    }

    public record Response(
        long Balance,
        string GameId,
        List<RollbackTransaction>? Transactions);

    public record RollbackTransaction(
        string ActionId,
        string TxId,
        DateTime ProcessedAt);

    public record RollbackAction(
        string Action,
        string ActionId,
        string OriginalActionId);
}