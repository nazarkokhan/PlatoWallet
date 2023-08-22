namespace Platipus.Wallet.Api.Application.Requests.Wallets.Uranus;

using Base;
using Data;
using FluentValidation;
using Results.ResultToResultMappers;
using Platipus.Wallet.Api.Application.Results.Uranus;
using Platipus.Wallet.Api.Application.Results.Uranus.WithData;
using Platipus.Wallet.Api.Application.Services.Wallet;

public sealed record UranusWithdrawRequest(
    string SessionToken,
    string PlayerId,
    string GameId,
    string TransactionId,
    string Amount,
    string Currency,
    string RoundId,
    string? Payload) : IUranusRequest, IRequest<IUranusResult<UranusSuccessResponse<UranusCommonDataWithTransaction>>>
{
    public sealed class Handler
        : IRequestHandler<UranusWithdrawRequest, IUranusResult<UranusSuccessResponse<UranusCommonDataWithTransaction>>>
    {
        private readonly IWalletService _walletService;

        public Handler(IWalletService walletService) => 
            _walletService = walletService;

        public async Task<IUranusResult<UranusSuccessResponse<UranusCommonDataWithTransaction>>> Handle(
            UranusWithdrawRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _walletService.BetAsync(
                request.SessionToken,
                request.RoundId,
                request.TransactionId,
                amount: decimal.Parse(request.Amount),
                currency: request.Currency,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToUranusFailureResult<UranusSuccessResponse<UranusCommonDataWithTransaction>>();

            var response = new UranusSuccessResponse<UranusCommonDataWithTransaction>(
                new UranusCommonDataWithTransaction(
                    walletResult.Data?.Currency,
                     walletResult.Data!.Balance, 
                    walletResult.Data.Transaction.Id));

            return UranusResultFactory.Success(response);
        }
    }
    
    public sealed class EvoplayWithdrawRequestValidator : AbstractValidator<UranusWithdrawRequest>
    {
        public EvoplayWithdrawRequestValidator()
        {
            RuleFor(x => x.SessionToken)
                .NotEmpty()
                .Length(1, 255);

            RuleFor(x => x.PlayerId)
                .NotEmpty()
                .Length(1, 255);

            RuleFor(x => x.GameId)
                .NotEmpty()
                .Length(1, 255);

            RuleFor(x => x.TransactionId)
                .NotEmpty()
                .Length(1, 255);

            RuleFor(x => x.Amount)
                .NotEmpty()
                .MinimumLength(1)
                .Matches(@"^\d+(\.\d{1,10})?$")
                .WithMessage("The amount should be a numeric string with up to 10 decimal points.");

            RuleFor(x => x.Currency)
                .NotEmpty()
                .Length(3, 4);

            RuleFor(x => x.RoundId)
                .NotEmpty()
                .MaximumLength(255);
        }
    }
}