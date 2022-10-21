namespace PlatipusWallet.Api.Application.Requests.JSysWallet;

using Base.Requests;
using Base.Responses.Databet;
using Domain.Entities;
using Infrastructure.Persistence;
using MediatR;
using Results.Common;
using Microsoft.EntityFrameworkCore;
using Results.Common.Result.Factories;
using Results.Common.Result.WithData;

public record DatabetBetResultRequest(
    string PlayerId,
    decimal Amount,
    string GameCode,
    string RoundId,
    string TransactionId,
    bool EndRound,
    string Hash) : DatabetBaseRequest(PlayerId, Hash), IRequest<IDatabetResult<DatabetBalanceResponse>>
{
    public class Handler : IRequestHandler<DatabetBetResultRequest, IDatabetResult<DatabetBalanceResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IDatabetResult<DatabetBalanceResponse>> Handle(
            DatabetBetResultRequest request,
            CancellationToken cancellationToken)
        {
            var round = await _context.Set<Round>()
                .Where(
                    r => r.Id == request.RoundId &&
                         r.User.UserName == request.PlayerId)
                .Include(r => r.User.Currency)
                .Include(r => r.Transactions)
                .FirstOrDefaultAsync(cancellationToken);

            if (round is null)
                return DatabetResultFactory.Failure<DatabetBalanceResponse>(DatabetErrorCode.RoundNotFound);

            if (round.Transactions.Any(t => t.Id == request.TransactionId))
                return DatabetResultFactory.Failure<DatabetBalanceResponse>(DatabetErrorCode.TransactionNotFound);

            if (round.Finished)
                return DatabetResultFactory.Failure<DatabetBalanceResponse>(DatabetErrorCode.RoundNotFound);

            round.User.Balance += request.Amount;
            if (request.EndRound)
                round.Finished = request.EndRound;

            var transaction = new Transaction
            {
                Id = request.TransactionId,
                Amount = request.Amount,
            };

            round.Transactions.Add(transaction);

            _context.Update(round);
            await _context.SaveChangesAsync(cancellationToken);

            var response = new DatabetBalanceResponse(round.User.UserName, round.User.Currency.Name, round.User.Balance);

            return DatabetResultFactory.Success(response);
        }
    }

    public override string GetSource()
    {
        return PlayerId + Amount + GameCode + RoundId + TransactionId + EndRound.ToString().ToLower();
    }
}