namespace Platipus.Wallet.Api.Application.Requests.Wallets.BetConstruct;

using Base;
using Base.Response;
using Results.BetConstruct.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;
using static Results.BetConstruct.BetconstructResultFactory;

public record BetConstructRollbackTransactionRequest(
        string Token,
        string TransactionId,
        string GameId)
    : IBetconstructRequest, IRequest<IBetconstructResult<BetconstructPlayResponse>>
{
    public class Handler : IRequestHandler<BetConstructRollbackTransactionRequest, IBetconstructResult<BetconstructPlayResponse>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<IBetconstructResult<BetconstructPlayResponse>> Handle(
            BetConstructRollbackTransactionRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _wallet.RollbackAsync(
                request.Token,
                request.TransactionId,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToBetConstructResult<BetconstructPlayResponse>();
            var data = walletResult.Data;

            var response = new BetconstructPlayResponse(
                data.Transaction.Id,
                data.Balance);

            return Success(response);
        }
    }
}