namespace Platipus.Wallet.Api.Application.Requests.Wallets.Evenbet;

using System.Text.Json.Serialization;
using Base;
using FluentValidation;
using Helpers.Common;
using Responses.Evenbet;
using Results.Evenbet.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;

public sealed record EvenbetDebitRequest(
        [property: JsonPropertyName("token")] string Token,
        [property: JsonPropertyName("gameId")] string GameId,
        [property: JsonPropertyName("endRound")] bool EndRound,
        [property: JsonPropertyName("roundId")] string RoundId,
        [property: JsonPropertyName("transactionId")] string TransactionId,
        [property: JsonPropertyName("amount")] int Amount)
    : IEvenbetRequest, IRequest<IEvenbetResult<EvenbetDebitResponse>>
{
    public sealed class Handler : IRequestHandler<EvenbetDebitRequest, IEvenbetResult<EvenbetDebitResponse>>
    {
        private readonly IWalletService _walletService;

        public Handler(IWalletService walletService)
        {
            _walletService = walletService;
        }

        public async Task<IEvenbetResult<EvenbetDebitResponse>> Handle(
            EvenbetDebitRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _walletService.BetAsync(
                request.Token,
                roundId: request.RoundId,
                transactionId: request.TransactionId,
                amount: MoneyHelper.ConvertFromCents(request.Amount),
                roundFinished: request.EndRound,
                cancellationToken: cancellationToken);
            
            if (walletResult.IsFailure)
                return walletResult.ToEvenbetFailureResult<EvenbetDebitResponse>();

            var data = walletResult.Data;

            var response = new EvenbetDebitResponse(
                (int)MoneyHelper.ConvertToCents(data.Balance),
                DateTimeOffset.Now.ToUnixTimeSeconds().ToString(),
                data.Transaction.Id);

            return walletResult.ToEvenbetResult(response);
        }
    }

    //TODO did you test it? Are you sure it is used and is useful to you?
    public sealed class EvenbetDebitRequestValidator : AbstractValidator<EvenbetDebitRequest>
    {
        public EvenbetDebitRequestValidator()
        {
            RuleFor(x => x.Token)
               .NotEmpty()
               .WithMessage("Token is required.");

            RuleFor(x => x.GameId)
               .NotEmpty()
               .WithMessage("GameId is required.");

            RuleFor(x => x.EndRound)
               .NotNull()
               .WithMessage("EndRound is required.");

            RuleFor(x => x.RoundId)
               .NotEmpty()
               .WithMessage("RoundId is required.");

            RuleFor(x => x.TransactionId)
               .NotEmpty()
               .WithMessage("TransactionId is required.");

            RuleFor(x => x.Amount)
               .GreaterThanOrEqualTo(0)
               .WithMessage("Amount must be greater than or equal 0.");
        }
    }
}