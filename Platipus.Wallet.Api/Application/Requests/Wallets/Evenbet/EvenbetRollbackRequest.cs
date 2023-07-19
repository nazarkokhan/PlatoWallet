namespace Platipus.Wallet.Api.Application.Requests.Wallets.Evenbet;

using Base;
using FluentValidation;
using Newtonsoft.Json;
using Responses.Evenbet;
using Responses.Evenbet.Base;
using Results.Evenbet.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;

public sealed record EvenbetRollbackRequest(
        [property: JsonProperty("token")] string Token,
        [property: JsonProperty("gameId")] string GameId,
        [property: JsonProperty("roundId")] string RoundId,
        [property: JsonProperty("transactionId")] string TransactionId,
        [property: JsonProperty("refTransactionId")] string RefTransactionId,
        [property: JsonProperty("amount")] decimal Amount)
    : IEvenbetRequest, IRequest<IEvenbetResult<EvenbetRollbackResponse>>
{
    public sealed class Handler : IRequestHandler<EvenbetRollbackRequest, IEvenbetResult<EvenbetRollbackResponse>>
    {
        private readonly IWalletService _walletService;

        public Handler(IWalletService walletService)
        {
            _walletService = walletService;
        }

        public async Task<IEvenbetResult<EvenbetRollbackResponse>> Handle(
            EvenbetRollbackRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _walletService.RollbackAsync(
                request.Token,
                roundId: request.RoundId,
                transactionId: request.RefTransactionId,
                amount: request.Amount,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure || walletResult.Data is null)
            {
                return walletResult.ToEvenbetFailureResult<EvenbetRollbackResponse>();
            }

            var data = walletResult.Data;

            var response = new EvenbetRollbackResponse(
                data!.Balance,
                DateTimeOffset.Now.ToUnixTimeSeconds().ToString(),
                data.Transaction.Id);

            return walletResult.ToEvenbetResult(response);
        }
    }

    public sealed class EvenbetRollbackRequestValidator : AbstractValidator<EvenbetRollbackRequest>
    {
        public EvenbetRollbackRequestValidator()
        {
            RuleFor(x => x.Token)
               .NotEmpty()
               .WithMessage("Token is required.");

            RuleFor(x => x.GameId)
               .NotEmpty()
               .WithMessage("GameId is required.");

            RuleFor(x => x.RefTransactionId)
               .NotEmpty()
               .WithMessage("RefTransactionId is required.");

            RuleFor(x => x.RoundId)
               .NotEmpty()
               .WithMessage("RoundId is required.");

            RuleFor(x => x.TransactionId)
               .NotEmpty()
               .WithMessage("TransactionId is required.");

            RuleFor(x => x.Amount)
               .NotEmpty()
               .WithMessage("Amount is required.")
               .GreaterThanOrEqualTo(0.0M)
               .WithMessage("Amount must be greater than or equal 0.");
        }
    }
}