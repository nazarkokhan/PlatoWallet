namespace Platipus.Wallet.Api.Application.Requests.Wallets.Hub88;

using Base;
using Base.Response;
using Results.Hub88;
using Results.Hub88.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;

public record Hub88GetBalanceRequest(
    string SupplierUser,
    string Token,
    string RequestUuid,
    int GameId,
    string GameCode) : IHub88BaseRequest, IRequest<IHub88Result<Hub88BalanceResponse>>
{
    public class Handler : IRequestHandler<Hub88GetBalanceRequest, IHub88Result<Hub88BalanceResponse>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<IHub88Result<Hub88BalanceResponse>> Handle(
            Hub88GetBalanceRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _wallet.GetBalanceAsync(
                request.Token,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToHub88Result<Hub88BalanceResponse>();
            var data = walletResult.Data;

            var response = new Hub88BalanceResponse(
                (int)(data.Balance * 100000),
                request.SupplierUser,
                request.RequestUuid,
                data.Currency);

            return Hub88ResultFactory.Success(response);
        }
    }
}