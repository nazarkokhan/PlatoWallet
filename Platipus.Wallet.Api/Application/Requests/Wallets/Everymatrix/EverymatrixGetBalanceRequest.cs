namespace Platipus.Wallet.Api.Application.Requests.Wallets.Everymatrix;

using Base;
using Base.Response;
using Extensions;
using Results.Everymatrix;
using Results.Everymatrix.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;
using Services.Wallet.DTOs;

public record EverymatrixGetBalanceRequest(
    string SupplierUser,
    Guid Token,
    string RequestUuid,
    int GameId,
    string GameCode) : IEverymatrixBaseRequest, IRequest<IEverymatrixResult<EverymatrixBalanceResponse>>
{
    public class Handler : IRequestHandler<EverymatrixGetBalanceRequest, IEverymatrixResult<EverymatrixBalanceResponse>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<IEverymatrixResult<EverymatrixBalanceResponse>> Handle(
            EverymatrixGetBalanceRequest request,
            CancellationToken cancellationToken)
        {
            var walletRequest = request.Map(r => new GetBalanceRequest(r.Token, r.SupplierUser));

            var walletResult = await _wallet.GetBalanceAsync(walletRequest, cancellationToken);
            if (walletResult.IsFailure)
                return walletResult.ToEverymatrixResult<EverymatrixBalanceResponse>();

            var response = walletResult.Data.Map(
                d => new EverymatrixBalanceResponse(
                    (int)(d.Balance * 100000),
                    request.SupplierUser,
                    request.RequestUuid,
                    d.Currency));

            return EverymatrixResultFactory.Success(response);
        }
    }
}