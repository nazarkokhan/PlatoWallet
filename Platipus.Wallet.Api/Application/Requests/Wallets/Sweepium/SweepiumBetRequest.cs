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
using Platipus.Wallet.Api.Application.Services.Wallet;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.Sweepium;

public sealed record SweepiumBetRequest(
        [property: DefaultValue("your session token")] string DateTime,
        [property: DefaultValue("requested API method parameters")] SweepiumBetData Data,
        [property: DefaultValue("your hash")] string Hash)
    : ISweepiumRequest, IRequest<ISweepiumResult<SweepiumCommonResponse>>
{
    public sealed class Handler
        : IRequestHandler<SweepiumBetRequest, ISweepiumResult<SweepiumCommonResponse>>
    {
        private readonly IWalletService _walletService;

        public Handler(IWalletService walletService) => 
            _walletService = walletService;

        public async Task<ISweepiumResult<SweepiumCommonResponse>> Handle(
            SweepiumBetRequest request,
            CancellationToken cancellationToken)
        {
            var data = request.Data;
            var amount = MoneyHelper.ConvertFromCents(data.BetAmount);
            var walletResult = await _walletService.BetAsync(
                data.Token,
                data.RoundId,
                data.TransactionId,
                amount,
                data.CurrencyId,
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
    
    //public sealed class EvoplayWithdrawRequestValidator : AbstractValidator<SweepiumWithdrawRequest>
    //{
    //    public EvoplayWithdrawRequestValidator()
    //    {
    //        RuleFor(x => x.SessionToken)
    //            .NotEmpty()
    //            .Length(1, 255);
//
    //        RuleFor(x => x.PlayerId)
    //            .NotEmpty()
    //            .Length(1, 255);
//
    //        RuleFor(x => x.GameId)
    //            .NotEmpty()
    //            .Length(1, 255);
//
    //        RuleFor(x => x.TransactionId)
    //            .NotEmpty()
    //            .Length(1, 255);
//
    //        RuleFor(x => x.Amount)
    //            .NotEmpty()
    //            .MinimumLength(1)
    //            .Matches(@"^\d+(\.\d{1,10})?$")
    //            .WithMessage("The amount should be a numeric string with up to 10 decimal points.");
//
    //        RuleFor(x => x.Currency)
    //            .NotEmpty()
    //            .Length(3, 4);
//
    //        RuleFor(x => x.RoundId)
    //            .NotEmpty()
    //            .MaximumLength(255);
    //    }
    //}
}