namespace Platipus.Wallet.Api.Application.Requests.Wallets.BetConstruct;

using Base;
using Base.Response;
using Results.BetConstruct.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;
using static Results.BetConstruct.BetConstructResultFactory;

public record BetConstructGetPlayerInfoRequest(GetPlayerInfoData Data, DateTime Time, string Hash)
    : IBetConstructBaseRequest<GetPlayerInfoData>, IRequest<IBetConstructResult<BetConstructGetPlayerInfoResponse>>
{
    public class Handler
        : IRequestHandler<BetConstructGetPlayerInfoRequest, IBetConstructResult<BetConstructGetPlayerInfoResponse>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<IBetConstructResult<BetConstructGetPlayerInfoResponse>> Handle(
            BetConstructGetPlayerInfoRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _wallet.GetBalanceAsync(
                request.Data.Token,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToBetConstructResult<BetConstructGetPlayerInfoResponse>();
            var data = walletResult.Data;

            var response = new BetConstructGetPlayerInfoResponse(
                true,
                null,
                null,
                data.Currency,
                data.Balance,
                data.Username,
                data.UserId);

            return Success(response);
        }
    }
}

public record GetPlayerInfoData(string Token) : IBetConstructDataRequest;