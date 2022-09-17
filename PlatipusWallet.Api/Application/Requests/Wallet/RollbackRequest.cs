namespace PlatipusWallet.Api.Application.Requests.Wallet;

using MediatR;
using Responses;
using Results.Common.Result;
using Results.Common.Result.WithData;

public record RollbackRequest(
    string SessionId,
    string User,
    string Currency,
    string Game,
    string RoundId,
    string TransactionId,
    string Amount) : IRequest<IResult<BalanceResponse>>
{
    public class Handler : IRequestHandler<RollbackRequest, IResult<BalanceResponse>>
    {
        public async Task<IResult<BalanceResponse>> Handle(
            RollbackRequest request,
            CancellationToken cancellationToken)
        {
            var result = (BalanceResponse) default;

            return ResultFactory.Success(result);
        }
    }
}