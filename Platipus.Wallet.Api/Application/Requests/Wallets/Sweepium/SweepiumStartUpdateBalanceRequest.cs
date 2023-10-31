using System.ComponentModel;
using System.Text.Json.Serialization;
using Platipus.Wallet.Api.Application.Helpers;
using Platipus.Wallet.Api.Application.Requests.Wallets.Evenbet;
using Platipus.Wallet.Api.Application.Requests.Wallets.Sweepium.Base;
using Platipus.Wallet.Api.Application.Requests.Wallets.Sweepium.Data;
using Platipus.Wallet.Api.Application.Responses.Evenbet;
using Platipus.Wallet.Api.Application.Responses.Sweepium;
using Platipus.Wallet.Api.Application.Results.Evenbet.WithData;
using Platipus.Wallet.Api.Application.Results.ResultToResultMappers;
using Platipus.Wallet.Api.Application.Results.Sweepium;
using Platipus.Wallet.Api.Application.Services.Wallet;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.Sweepium;

public sealed record SweepiumStartUpdateBalanceRequest(
        [property: DefaultValue("your session token")] string DateTime,
        [property: DefaultValue("requested API method parameters")] SweepiumStartUpdateBalanceData Data,
        [property: DefaultValue("your hash")] string Hash)
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
                request.Data.Token,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure || walletResult.Data is null)
            {
                return walletResult.ToSweepiumErrorResult<SweepiumStartUpdateBalanceResponse>();
            }

            var data = walletResult.Data;
            var totalBalance = MoneyHelper.ConvertToCents(data.Balance);

            var response = new SweepiumStartUpdateBalanceResponse(walletResult.IsSuccess, data.Currency, Convert. ToInt32(totalBalance), data.UserId);

            return walletResult.ToSweepiumResult(response);
        }
    }
}