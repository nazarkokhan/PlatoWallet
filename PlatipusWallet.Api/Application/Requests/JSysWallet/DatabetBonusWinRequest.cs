namespace PlatipusWallet.Api.Application.Requests.JSysWallet;

using Base.Requests;
using Microsoft.EntityFrameworkCore;
using Base.Responses.Databet;
using Domain.Entities;
using Infrastructure.Persistence;
using MediatR;
using Results.Common;
using Results.Common.Result.Factories;
using Results.Common.Result.WithData;

public record DatabetBonusWinRequest(
    string PlayerId,
    decimal Amount,
    string GameCode,
    string RoundId,
    string TransactionId,
    string Hash) : DatabetBaseRequest(PlayerId, Hash), IRequest<IDatabetResult<DatabetBalanceResponse>>
{
    public class Handler : IRequestHandler<DatabetBonusWinRequest, IDatabetResult<DatabetBalanceResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IDatabetResult<DatabetBalanceResponse>> Handle(
            DatabetBonusWinRequest request,
            CancellationToken cancellationToken)
        {
            var award = await _context.Set<Award>()
                .Where(a => a.Id == "") //TODO
                .Include(a => a.User)
                .Include(a => a.AwardRound!.Round)
                .FirstOrDefaultAsync(cancellationToken);

            if (award is null || award.User.UserName != request.PlayerId)
                return DatabetResultFactory.Failure<DatabetBalanceResponse>(DatabetErrorCode.PlayerNotFound);

            if (award.AwardRound is not null)
                return DatabetResultFactory.Failure<DatabetBalanceResponse>(DatabetErrorCode.SystemError);

            var round = await _context.Set<Round>()
                .Where(r => r.Id == request.RoundId)
                .Include(a => a.Transactions)
                .FirstOrDefaultAsync(cancellationToken);

            if (round is not null)
                return DatabetResultFactory.Failure<DatabetBalanceResponse>(DatabetErrorCode.RoundNotFound);

            round = new Round
            {
                Id = request.RoundId,
                Finished = true,
                Transactions = new List<Transaction>
                {
                    new()
                    {
                        Id = request.TransactionId,
                        Amount = request.Amount
                    }
                },
                AwardRound = new AwardRound
                {
                    Award = award
                }
            };
            _context.Add(round);

            var user = award.User;

            user.Rounds.Add(round);
            user.Balance += request.Amount;
            _context.Update(user);

            await _context.SaveChangesAsync(cancellationToken);

            var response = new DatabetBalanceResponse(user.UserName, user.Currency.Name, user.Balance);

            return DatabetResultFactory.Success(response);
        }
    }
    
    public override string GetSource()
    {
        return PlayerId + Amount + GameCode + RoundId + TransactionId;
    }
}