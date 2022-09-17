namespace PlatipusWallet.Api.Application.Requests.Wallet;

using Infrastructure.Persistence;
using MediatR;
using Responses;
using Results.Common.Result;
using Results.Common.Result.WithData;

public record GetBalanceRequest(
    string SessionId,
    string User,
    string Currency,
    string Game) : IRequest<IResult<BalanceResponse>>
{
    public class Handler : IRequestHandler<GetBalanceRequest, IResult<BalanceResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IResult<BalanceResponse>> Handle(
            GetBalanceRequest request,
            CancellationToken cancellationToken)
        {
            var result = (BalanceResponse) default;

            return ResultFactory.Success(result);
        }
    }
}