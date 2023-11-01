using System.ComponentModel;
using System.Text.Json.Serialization;
using Bogus.DataSets;
using FluentValidation;
using Humanizer;
using Newtonsoft.Json;
using Platipus.Wallet.Api.Application.Helpers;
using Platipus.Wallet.Api.Application.Requests.Wallets.Sweepium.Base;
using Platipus.Wallet.Api.Application.Requests.Wallets.Sweepium.Data;
using Platipus.Wallet.Api.Application.Responses.Sweepium;
using Platipus.Wallet.Api.Application.Responses.Sweepium.Base;
using Platipus.Wallet.Api.Application.Results.ResultToResultMappers;
using Platipus.Wallet.Api.Application.Results.Sweepium;
using Platipus.Wallet.Api.Application.Results.Sweepium.WithData;
using Platipus.Wallet.Api.Application.Services.Wallet;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.Sweepium;

public sealed record SweepiumRollbackRequest(
        [property: DefaultValue("your session token")] string DateTime,
        [property: DefaultValue("requested API method parameters")] SweepiumWinData Data,
        [property: DefaultValue("your hash")] string Hash)
    : ISweepiumRequest, IRequest<ISweepiumResult<SweepiumCommonResponse>>
{
    public sealed class Handler
        : IRequestHandler<SweepiumRollbackRequest, ISweepiumResult<SweepiumCommonResponse>>
    {
        private readonly IWalletService _walletService;

        public Handler(IWalletService walletService) => 
            _walletService = walletService;

        public async Task<ISweepiumResult<SweepiumCommonResponse>> Handle(
            SweepiumRollbackRequest request,
            CancellationToken cancellationToken)
        {
            var data = request.Data;
            var walletResult = await _walletService.RollbackAsync(
                data.Token,
                data.RoundId,
                data.TransactionId,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToSweepiumErrorResult<SweepiumErrorResponse>();

            var response = new SweepiumSuccessResponse(
                    walletResult.IsSuccess,
                    walletResult.Data.Transaction.Id,
                    (int)MoneyHelper.ConvertToCents(walletResult.Data!.Balance)
                    );

            return SweepiumResultFactory.Success(response);
        }
    }
}