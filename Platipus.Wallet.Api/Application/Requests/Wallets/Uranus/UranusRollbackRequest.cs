namespace Platipus.Wallet.Api.Application.Requests.Wallets.Uranus;

using Base;
using Data;
using FluentValidation;
using Platipus.Wallet.Api.Application.Results.ResultToResultMappers;
using Platipus.Wallet.Api.Application.Results.Uranus;
using Platipus.Wallet.Api.Application.Results.Uranus.WithData;
using Platipus.Wallet.Api.Application.Services.Wallet;

public sealed record UranusRollbackRequest(
    string SessionToken, 
    string PlayerId, 
    string GameId,
    string RollbackTransactionId,
    string RoundId,
    string TransactionId, 
    string Amount, 
    string Currency, 
    string? Payload) : IUranusRequest, IRequest<IUranusResult<UranusSuccessResponse<UranusCommonDataWithTransaction>>>
{
    public sealed class Handler 
        : IRequestHandler<UranusRollbackRequest, IUranusResult<UranusSuccessResponse<UranusCommonDataWithTransaction>>>
    {
        private readonly IWalletService _walletService;

        public Handler(IWalletService walletService) => 
            _walletService = walletService;

        public async Task<IUranusResult<UranusSuccessResponse<UranusCommonDataWithTransaction>>> Handle(
            UranusRollbackRequest request, 
            CancellationToken cancellationToken)
        {
            var walletResult = await _walletService.RollbackAsync(
                request.SessionToken,
                request.RoundId,
                request.RollbackTransactionId,
                amount: int.Parse(request.Amount),
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure || walletResult.Data is null)
                return walletResult.ToUranusFailureResult<UranusSuccessResponse<UranusCommonDataWithTransaction>>();
            var response = new UranusSuccessResponse<UranusCommonDataWithTransaction>(
             new UranusCommonDataWithTransaction(
                 walletResult.Data?.Currency, 
                 walletResult.Data!.Balance,
                 walletResult.Data.Transaction.Id
                 ));
            return UranusResultFactory.Success(response);
        }
    }
    
    public sealed class EvoplayRollbackRequestValidator : AbstractValidator<UranusRollbackRequest>
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