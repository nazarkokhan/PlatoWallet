namespace Platipus.Wallet.Api.Application.Requests.Wallets.Evoplay;

using Base;
using Data;
using FluentValidation;
using Results.Evoplay;
using Results.Evoplay.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;

public sealed record EvoplayRollbackRequest(
    string SessionToken, 
    string PlayerId, 
    string GameId,
    string RollbackTransactionId,
    string RoundId,
    string TransactionId, 
    string Amount, 
    string Currency, 
    string? Payload) : IEvoplayRequest, IRequest<IEvoplayResult<EvoplaySuccessResponse<EvoplayCommonDataWithTransaction>>>
{
    public sealed class Handler 
        : IRequestHandler<EvoplayRollbackRequest, IEvoplayResult<EvoplaySuccessResponse<EvoplayCommonDataWithTransaction>>>
    {
        private readonly IWalletService _walletService;

        public Handler(IWalletService walletService) => 
            _walletService = walletService;

        public async Task<IEvoplayResult<EvoplaySuccessResponse<EvoplayCommonDataWithTransaction>>> Handle(
            EvoplayRollbackRequest request, 
            CancellationToken cancellationToken)
        {
            var walletResult = await _walletService.RollbackAsync(
                request.SessionToken,
                request.RoundId,
                request.TransactionId,
                amount: int.Parse(request.Amount),
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure || walletResult.Data is null)
                return walletResult.ToEvoplayFailureResult<EvoplaySuccessResponse<EvoplayCommonDataWithTransaction>>();
            var response = new EvoplaySuccessResponse<EvoplayCommonDataWithTransaction>(
             new EvoplayCommonDataWithTransaction(
                 walletResult.Data?.Currency, 
                 walletResult.Data!.Balance,
                 walletResult.Data.Transaction.Id
                 ));
            return EvoplayResultFactory.Success(response);
        }
    }
    
    public sealed class EvoplayRollbackRequestValidator : AbstractValidator<EvoplayRollbackRequest>
    {
        public EvoplayRollbackRequestValidator()
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

            RuleFor(x => x.RollbackTransactionId)
                .NotEmpty()
                .Length(1, 255);

            RuleFor(x => x.Amount)
                .Cascade(CascadeMode.Stop)
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