namespace Platipus.Wallet.Api.Application.Requests.Wallets.Hub88;

using Base;
using Base.Response;
using Extensions;
using Results.Hub88;
using Results.Hub88.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;
using Services.Wallet.DTOs;

public record Hub88GetBalanceRequest(
    string SupplierUser,
    Guid Token,
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
            var walletRequest = request.Map(r => new GetBalanceRequest(r.Token, r.SupplierUser));

            var walletResult = await _wallet.GetBalanceAsync(walletRequest, cancellationToken);
            if (walletResult.IsFailure)
                return walletResult.ToHub88Result<Hub88BalanceResponse>();

            var response = walletResult.Data.Map(
                d => new Hub88BalanceResponse(
                    (int)(d.Balance * 100000),
                    request.SupplierUser,
                    request.RequestUuid,
                    d.Currency));

            return Hub88ResultFactory.Success(response);
        }
    }
}