namespace Platipus.Wallet.Api.Application.Requests.Wallets.Hub88;

using Base;
using Base.Response;
using Extensions;
using Results.Hub88;
using Results.Hub88.WithData;
using Results.Hub88.WithData.Mappers;
using Services.Wallet;
using Services.Wallet.DTOs;

public record Hub88GetBalanceRequest(
    string SupplierUser,
    string Token,
    string RequestUuid,
    int GameId,
    string GameCode) : Hub88BaseRequest(SupplierUser, Token, RequestUuid), IRequest<IHub88Result<Hub88BalanceResponse>>
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
            var walletRequest = request.Map(r => new GetBalanceRequest(new Guid(r.Token), r.SupplierUser));

            var walletResult = await _wallet.GetBalanceAsync(walletRequest, cancellationToken);
            if (walletResult.IsFailure)
                walletResult.ToHub88Result();

            var response = walletResult.Data.Map(
                d => new Hub88BalanceResponse(
                    (int) (d.Balance * 100),
                    request.SupplierUser,
                    request.RequestUuid,
                    d.Currency));

            return Hub88ResultFactory.Success(response);
        }
    }
}