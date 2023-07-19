namespace Platipus.Wallet.Api.Application.Requests.Wallets.Evenbet;

using Base;
using FluentValidation;
using Newtonsoft.Json;
using Responses.Evenbet;
using Responses.Evenbet.Base;
using Results.Evenbet.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;

public sealed record EvenbetCreditRequest(
        [property: JsonProperty("token")] string Token,
        [property: JsonProperty("gameId")] string GameId,
        [property: JsonProperty("endRound")] bool EndRound,
        [property: JsonProperty("roundId")] string RoundId,
        [property: JsonProperty("transactionId")] string TransactionId,
        [property: JsonProperty("amount")] decimal Amount)
    : IEvenbetRequest, IRequest<IEvenbetResult<EvenbetCreditResponse>>
{
    public sealed class Handler : IRequestHandler<EvenbetCreditRequest, IEvenbetResult<EvenbetCreditResponse>>
    {
        private readonly IWalletService _walletService;

        public Handler(IWalletService walletService)
        {
            _walletService = walletService;
        }

        public async Task<IEvenbetResult<EvenbetCreditResponse>> Handle(
            EvenbetCreditRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _walletService.WinAsync(
                request.Token,
                roundId: request.RoundId,
                transactionId: request.TransactionId,
                amount: request.Amount,
                roundFinished: request.EndRound,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure || walletResult.Data is null)
            {
                walletResult.ToEvenbetFailureResult<EvenbetFailureResponse>();
            }
            
            var data = walletResult.Data;

            var response = new EvenbetCreditResponse(
                data!.Balance,
                DateTimeOffset.Now.ToUnixTimeSeconds().ToString(),
                data.Transaction.Id);

            return walletResult.ToEvenbetResult(response);
        }
    }

    public sealed class EvenbetCreditRequestValidator : AbstractValidator<EvenbetCreditRequest>
    {
        public EvenbetCreditRequestValidator()
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
               .GreaterThanOrEqualTo(0.0M)
               .WithMessage("Amount must be greater than or equal 0.");
        }
    }
}