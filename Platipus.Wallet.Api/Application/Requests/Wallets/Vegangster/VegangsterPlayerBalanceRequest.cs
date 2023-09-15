namespace Platipus.Wallet.Api.Application.Requests.Wallets.Vegangster;

using Base;
using Helpers;
using Responses.Vegangster;
using Results.ResultToResultMappers;
using Results.Vegangster;
using Results.Vegangster.WithData;
using Services.Wallet;

public sealed record VegangsterPlayerBalanceRequest(
        string Token, 
        string PlayerId)
    : IVegangsterBaseRequest, IRequest<IVegangsterResult<VegangsterPlayerBalanceResponse>>
{
    public sealed class Handler
        : IRequestHandler<VegangsterPlayerBalanceRequest, IVegangsterResult<VegangsterPlayerBalanceResponse>>
    {
        private readonly IWalletService _walletService;

        public Handler(IWalletService walletService)
        {
            _walletService = walletService;
        }

        public async Task<IVegangsterResult<VegangsterPlayerBalanceResponse>> Handle(
            VegangsterPlayerBalanceRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _walletService.GetBalanceAsync(
                request.Token,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToVegangsterFailureResult<VegangsterPlayerBalanceResponse>();

            var data = walletResult.Data;

            var response = new VegangsterPlayerBalanceResponse(
                VegangsterResponseStatus.OK.ToString(),
                data.Currency,
                (int)MoneyHelper.ConvertToCents(data.Balance));

            return walletResult.ToVegangsterResult(response);
        }
    }
}