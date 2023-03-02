namespace Platipus.Wallet.Api.Application.Requests.Wallets.Everymatrix;

using Base;
using Base.Response;
using Results.Everymatrix;
using Results.Everymatrix.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;

public record EverymatrixGetBalanceRequest(
    string Token,
    string Currency,
    string Hash) : IRequest<IEverymatrixResult<EverymatrixBalanceResponse>>, IEveryMatrixRequest
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
            var walletResult = await _wallet.GetBalanceAsync(
                request.Token,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToEverymatrixResult<EverymatrixBalanceResponse>();
            var data = walletResult.Data;

            var response = new EverymatrixBalanceResponse(data.Balance, data.Currency);

            return EverymatrixResultFactory.Success(response);
        }
    }
}