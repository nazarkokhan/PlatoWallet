namespace Platipus.Wallet.Api.Application.Requests.Wallets.Evenbet;

using Base;
using FluentValidation;
using Helpers.Evenbet;
using Newtonsoft.Json;
using Responses.Evenbet;
using Responses.Evenbet.Base;
using Results.Evenbet.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;

public sealed record EvenbetDebitRequest(
        [property: JsonProperty("token")] string Token,
        [property: JsonProperty("gameId")] string GameId,
        [property: JsonProperty("endRound")] bool EndRound,
        [property: JsonProperty("roundId")] string RoundId,
        [property: JsonProperty("transactionId")] string TransactionId,
        [property: JsonProperty("amount")] int Amount)
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
                amount: EvenbetMoneyHelper.ConvertToWallet(request.Amount),
                roundFinished: request.EndRound,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure || walletResult.Data is null)
            {
                return walletResult.ToEvenbetFailureResult<EvenbetDebitResponse>();
            }

            var data = walletResult.Data;

            var response = new EvenbetDebitResponse(
                EvenbetMoneyHelper.ConvertFromWallet(data!.Balance),
                DateTimeOffset.Now.ToUnixTimeSeconds().ToString(),
                data.Transaction.Id);

            return walletResult.ToEvenbetResult(response);
        }
    }

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
               .NotEmpty()
               .WithMessage("Amount is required.")
               .GreaterThan(0)
               .WithMessage("Amount must be greater than or equal 0.");
        }
    }
}