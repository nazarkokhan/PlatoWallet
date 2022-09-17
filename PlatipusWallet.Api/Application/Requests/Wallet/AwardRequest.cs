namespace PlatipusWallet.Api.Application.Requests.Wallet;

using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Responses;
using Results.Common.Result;
using Results.Common.Result.WithData;

public record AwardRequest(
    string SessionId,
    string User,
    string Currency,
    string Game,
    string RoundId,
    string TransactionId,
    string Amount,
    string AwardId) : IRequest<IResult<BalanceResponse>>
{
    public class Handler : IRequestHandler<AwardRequest, IResult<BalanceResponse>>
    {
        private readonly WalletDbContext _dbContext;

        public Handler(WalletDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IResult<BalanceResponse>> Handle(
            AwardRequest request,
            CancellationToken cancellationToken)
        {
            var x = await _dbContext.Users.FirstOrDefaultAsync(cancellationToken);

            var result = (BalanceResponse) default!;

            return ResultFactory.Success(result);
        }
    }
}