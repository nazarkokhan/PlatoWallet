namespace Platipus.Wallet.Api.Application.Requests.Wallets.BetConstruct;

using Base;
using Base.Response;
using Results.BetConstruct;
using Results.BetConstruct.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;

public record BetConstructDepositRequest(
        DepositData Data,
        DateTime Time,
        string Hash)
    : IBetConstructBaseRequest<DepositData>, IRequest<IBetConstructResult<BetConstructBaseResponse>>
{
    public class Handler : IRequestHandler<BetConstructDepositRequest, IBetConstructResult<BetConstructBaseResponse>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<IBetConstructResult<BetConstructBaseResponse>> Handle(
            BetConstructDepositRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _wallet.WinAsync(
                request.Data.Token,
                request.Data.RoundId,
                request.Data.TransactionId,
                request.Data.BetAmount,
                currency: request.Data.CurrencyId,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToBetConstructResult<BetConstructBaseResponse>();
            var data = walletResult.Data;

            var response = new BetConstructBaseResponse(
                true,
                null,
                null,
                data.Transaction.Id,
                data.Balance);

            return BetConstructResultFactory.Success(response);
        }
    }
}

public record DepositData(
    string Token,
    string TransactionId,
    string? RoundId,
    string GameId,
    string CurrencyId,
    decimal BetAmount,
    string BetInfo) : IBetConstructDataRequest;