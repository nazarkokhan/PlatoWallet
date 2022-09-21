namespace PlatipusWallet.Api.Application.Requests.Wallet;

using Base;
using Infrastructure.Persistence;
using MediatR;
using Responses;
using Results.Common.Result;
using Results.Common.Result.WithData;

public record BetRequest(
    string SessionId,
    string User,
    string Currency,
    string Game,
    string RoundId,
    string TransactionId,
    string Finished,
    string Amount) : BaseRequest(SessionId), IRequest<IResult<BalanceResponse>>
{
    public class Handler : IRequestHandler<BetRequest, IResult<BalanceResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IResult<BalanceResponse>> Handle(
            BetRequest request,
            CancellationToken cancellationToken)
        {
            var result = (BalanceResponse) default;

            return ResultFactory.Success(result);
        }
    }
}