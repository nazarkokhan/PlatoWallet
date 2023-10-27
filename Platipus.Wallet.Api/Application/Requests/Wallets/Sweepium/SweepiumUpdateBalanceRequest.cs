using System.Text.Json.Serialization;
using Platipus.Wallet.Api.Application.Helpers;
using Platipus.Wallet.Api.Application.Requests.Wallets.Evenbet;
using Platipus.Wallet.Api.Application.Requests.Wallets.Sweepium.Base;
using Platipus.Wallet.Api.Application.Responses.Evenbet;
using Platipus.Wallet.Api.Application.Responses.Sweepium;
using Platipus.Wallet.Api.Application.Results.Evenbet.WithData;
using Platipus.Wallet.Api.Application.Results.ResultToResultMappers;
using Platipus.Wallet.Api.Application.Results.Sweepium;
using Platipus.Wallet.Api.Application.Services.Wallet;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.Sweepium;

public sealed record SweepiumUpdateBalanceRequest([property: JsonPropertyName("token")] string Token)
    : ISweepiumRequest, IRequest<ISweepiumResult<SweepiumUpdateBalanceResponse>>
{
    public sealed class
        Handler : IRequestHandler<SweepiumUpdateBalanceRequest, ISweepiumResult<SweepiumUpdateBalanceResponse>>
    {
        private readonly IWalletService _walletService;

        public Handler(IWalletService walletService)
        {
            _walletService = walletService;
        }

        public async Task<ISweepiumResult<SweepiumUpdateBalanceResponse>> Handle(
            SweepiumUpdateBalanceRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _walletService.GetBalanceAsync(
                request.Token,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure || walletResult.Data is null)
            {
                return walletResult.ToSweepiumFailureResult<SweepiumUpdateBalanceResponse>();
            }

            var data = walletResult.Data;

            var response = new SweepiumUpdateBalanceResponse();

            return walletResult.ToSweepiumResult(response);
        }
    }
}