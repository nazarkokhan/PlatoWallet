namespace PlatipusWallet.Api.Application.Requests.Wallet;

using MediatR;
using Responses;
using Results.Common.Result;
using Results.Common.Result.WithData;

public record WinRequest(
    string SessionId,
    string User,
    string Currency,
    string Game,
    string RoundId,
    string TransactionId,
    string Finished,
    string Amount) : IRequest<IResult<BalanceResponse>>
{
    public class Handler : IRequestHandler<WinRequest, IResult<BalanceResponse>>
    {
        public async Task<IResult<BalanceResponse>> Handle(
            WinRequest request,
            CancellationToken cancellationToken)
        {
            var result = (BalanceResponse) default;

            return ResultFactory.Success(result);
        }
    }
}