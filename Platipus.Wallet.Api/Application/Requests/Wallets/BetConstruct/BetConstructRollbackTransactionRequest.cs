// ReSharper disable NotAccessedPositionalProperty.Global
namespace Platipus.Wallet.Api.Application.Requests.Wallets.BetConstruct;

using Base;
using Base.Response;
using Results.BetConstruct.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;
using static Results.BetConstruct.BetConstructResultFactory;

public record BetConstructRollbackTransactionRequest(RollbackData Data, DateTime Time, string Hash)
    : IBetConstructBaseRequest<RollbackData>, IRequest<IBetConstructResult<BetConstructBaseResponse>>
{
    public class Handler : IRequestHandler<BetConstructRollbackTransactionRequest, IBetConstructResult<BetConstructBaseResponse>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<IBetConstructResult<BetConstructBaseResponse>> Handle(
            BetConstructRollbackTransactionRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _wallet.RollbackAsync(
                request.Data.Token,
                request.Data.TransactionId,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToBetConstructResult<BetConstructBaseResponse>();
            var data = walletResult.Data;

            var response = new BetConstructBaseResponse(
                true,
                null,
                null,
                long.Parse(data.Transaction.Id), //TODO transaction.id is guid but required long
                data.Balance);

            return Success(response);
        }
    }
}

public record RollbackData(
    string Token,
    string TransactionId,
    string GameId
) : IBetConstructDataRequest;