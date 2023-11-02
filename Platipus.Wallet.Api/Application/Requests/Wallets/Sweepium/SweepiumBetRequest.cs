using System.ComponentModel;
using Platipus.Wallet.Api.Application.Helpers;
using Platipus.Wallet.Api.Application.Requests.Wallets.Sweepium.Base;
using Platipus.Wallet.Api.Application.Responses.Sweepium;
using Platipus.Wallet.Api.Application.Responses.Sweepium.Base;
using Platipus.Wallet.Api.Application.Results.ResultToResultMappers;
using Platipus.Wallet.Api.Application.Results.Sweepium;
using Platipus.Wallet.Api.Application.Results.Sweepium.WithData;
using Platipus.Wallet.Api.Application.Services.Wallet;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.Sweepium;

public sealed record SweepiumBetRequest(
        string Token,
        string TransactionId,
        string RoundId,
        string GameId,
        string CurrencyId,
        decimal BetAmount)
    : ISweepiumRequest, IRequest<ISweepiumResult<SweepiumSuccessResponse>>
{
    public sealed class Handler
        : IRequestHandler<SweepiumBetRequest, ISweepiumResult<SweepiumSuccessResponse>>
    {
        private readonly IWalletService _walletService;

        public Handler(IWalletService walletService) => 
            _walletService = walletService;

        public async Task<ISweepiumResult<SweepiumSuccessResponse>> Handle(
            SweepiumBetRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _walletService.BetAsync(
                request.Token,
                request.RoundId,
                request.TransactionId,
                request.BetAmount,
                request.CurrencyId,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToSweepiumResult<SweepiumSuccessResponse>();

            var data = walletResult.Data;

            var response = new SweepiumSuccessResponse(
                    data.Transaction.Id,
                    data.Balance);

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