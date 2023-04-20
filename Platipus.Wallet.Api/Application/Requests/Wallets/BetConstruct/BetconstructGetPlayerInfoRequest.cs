namespace Platipus.Wallet.Api.Application.Requests.Wallets.BetConstruct;

using Base;
using Base.Response;
using Results.BetConstruct.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;
using static Results.BetConstruct.BetconstructResultFactory;

public record BetconstructGetPlayerInfoRequest(string Token)
    : IBetconstructRequest, IRequest<IBetconstructResult<BetconstructGetPlayerInfoRequest.GetPlayerInfoResponse>>
{
    public class Handler : IRequestHandler<BetconstructGetPlayerInfoRequest, IBetconstructResult<GetPlayerInfoResponse>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<IBetconstructResult<GetPlayerInfoResponse>> Handle(
            BetconstructGetPlayerInfoRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _wallet.GetBalanceAsync(
                request.Token,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToBetConstructResult<GetPlayerInfoResponse>();
            var data = walletResult.Data;

            var response = new GetPlayerInfoResponse(
                data.Currency,
                data.Balance,
                data.Username,
                data.UserId);

            return Success(response);
        }
    }

    public record GetPlayerInfoResponse(
        string CurrencyId,
        decimal TotalBalance,
        string NickName,
        int UserId,
        int Gender = 1,
        string Country = "Ukraine") : BetconstructBaseResponse;
}