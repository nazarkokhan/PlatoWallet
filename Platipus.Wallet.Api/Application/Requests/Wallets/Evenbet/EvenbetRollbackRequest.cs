namespace Platipus.Wallet.Api.Application.Requests.Wallets.Evenbet;

using System.Text.Json.Serialization;
using Base;
using FluentValidation;
using Helpers.Common;
using Responses.Evenbet;
using Results.Evenbet.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;

public sealed record EvenbetRollbackRequest(
        [property: JsonPropertyName("token")] string Token,
        [property: JsonPropertyName("gameId")] string GameId,
        [property: JsonPropertyName("roundId")] string RoundId,
        [property: JsonPropertyName("transactionId")] string TransactionId,
        [property: JsonPropertyName("refTransactionId")] string RefTransactionId,
        [property: JsonPropertyName("amount")] int Amount)
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
                amount: MoneyHelper.ConvertFromCents(request.Amount),
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure || walletResult.Data is null)
            {
                return walletResult.ToEvenbetFailureResult<EvenbetRollbackResponse>();
            }

            var data = walletResult.Data;

            var response = new EvenbetRollbackResponse(
                (int)MoneyHelper.ConvertToCents(data!.Balance),
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
               .GreaterThanOrEqualTo(0)
               .WithMessage("Amount must be greater than or equal 0.");
        }
    }
}