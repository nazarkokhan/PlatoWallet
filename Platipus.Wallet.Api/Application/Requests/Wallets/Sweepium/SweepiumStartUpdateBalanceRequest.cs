using Platipus.Wallet.Api.Application.Helpers;
using Platipus.Wallet.Api.Application.Requests.Wallets.Sweepium.Base;
using Platipus.Wallet.Api.Application.Responses.Sweepium;
using Platipus.Wallet.Api.Application.Results.ResultToResultMappers;
using Platipus.Wallet.Api.Application.Results.Sweepium.WithData;
using Platipus.Wallet.Api.Application.Services.Wallet;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.Sweepium;

public sealed record SweepiumStartUpdateBalanceRequest(string Token)
    : ISweepiumRequest, IRequest<ISweepiumResult<SweepiumStartUpdateBalanceResponse>>
{
    public sealed class
        Handler : IRequestHandler<SweepiumStartUpdateBalanceRequest, ISweepiumResult<SweepiumStartUpdateBalanceResponse>>
    {
        private readonly IWalletService _walletService;

        public Handler(IWalletService walletService)
        {
            _walletService = walletService;
        }

        public async Task<ISweepiumResult<SweepiumStartUpdateBalanceResponse>> Handle(
            SweepiumStartUpdateBalanceRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _walletService.GetBalanceAsync(
                request.Token,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToSweepiumResult<SweepiumStartUpdateBalanceResponse>();

            var data = walletResult.Data;

            var response = new SweepiumStartUpdateBalanceResponse(
                data.Currency,
                (int)MoneyHelper.ConvertToCents(data.Balance),
                data.UserId);

            return walletResult.ToSweepiumResult(response);
        }
    }
}