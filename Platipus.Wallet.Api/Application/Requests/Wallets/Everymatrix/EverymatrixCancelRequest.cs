namespace Platipus.Wallet.Api.Application.Requests.Wallets.Everymatrix;

using Base;
using Base.Response;
using Results.Everymatrix;
using Results.Everymatrix.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;

public record EverymatrixCancelRequest(
    string Token,
    string ExternalId,
    string CanceledExternalId,
    string Hash) : IRequest<IEverymatrixResult<EverymatrixBalanceResponse>>, IEveryMatrixRequest
{
    public class Handler : IRequestHandler<EverymatrixCancelRequest, IEverymatrixResult<EverymatrixBalanceResponse>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<IEverymatrixResult<EverymatrixBalanceResponse>> Handle(
            EverymatrixCancelRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _wallet.RollbackAsync(
                request.Token,
                request.ExternalId,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToEverymatrixResult<EverymatrixBalanceResponse>();
            var data = walletResult.Data;

            var response = new EverymatrixBalanceResponse(data.Balance, data.Currency);

            return EverymatrixResultFactory.Success(response);
        }
    }
}