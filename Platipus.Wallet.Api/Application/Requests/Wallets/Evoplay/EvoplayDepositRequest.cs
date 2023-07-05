namespace Platipus.Wallet.Api.Application.Requests.Wallets.Evoplay;

using Base;
using Data;
using FluentValidation;
using Results.Evoplay;
using Results.Evoplay.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;

public sealed record EvoplayDepositRequest(
        string SessionToken,
        string PlayerId,
        string GameId,
        string TransactionId,
        string Amount,
        string Currency,
        string RoundId,
        bool RoundEnd,
        string? Payload)
    : IEvoplayRequest, IRequest<IEvoplayResult<EvoplaySuccessResponse<EvoplayCommonDataWithTransaction>>>
{
    public sealed class Handler : IRequestHandler<EvoplayDepositRequest,
        IEvoplayResult<EvoplaySuccessResponse<EvoplayCommonDataWithTransaction>>>
    {
        private readonly IWalletService _walletService;

        public Handler(IWalletService walletService) => 
            _walletService = walletService;

        public async Task<IEvoplayResult<EvoplaySuccessResponse<EvoplayCommonDataWithTransaction>>> Handle(
            EvoplayDepositRequest request, CancellationToken cancellationToken)
        {
            var walletResult = await _walletService.WinAsync(
                request.SessionToken,
                request.RoundId,
                request.TransactionId,
                decimal.Parse(request.Amount), 
                currency: request.Currency,
                roundFinished: request.RoundEnd,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToEvoplayFailureResult<EvoplaySuccessResponse<EvoplayCommonDataWithTransaction>>();

            var response = new EvoplaySuccessResponse<EvoplayCommonDataWithTransaction>(
                new EvoplayCommonDataWithTransaction(
                    walletResult.Data?.Currency, 
                    walletResult.Data!.Balance, 
                    walletResult.Data.Transaction.Id)
            );
            return EvoplayResultFactory.Success(response);
        }
    }

    public sealed class EvoplayDepositRequestValidator : AbstractValidator<EvoplayDepositRequest>
    {
        public EvoplayDepositRequestValidator()
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

            RuleFor(x => x.RoundEnd)
                .NotNull();
        }
    }
}