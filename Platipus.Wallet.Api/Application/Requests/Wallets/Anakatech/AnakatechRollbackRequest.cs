namespace Platipus.Wallet.Api.Application.Requests.Wallets.Anakatech;

using Base;
using Enums;
using FluentValidation;
using Helpers.Common;
using Newtonsoft.Json;
using Responses.Anakatech;
using Results.Anakatech.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;

public sealed record AnakatechRollbackRequest(
        [property: JsonProperty("secret")] string Secret,
        [property: JsonProperty("sessionId")] string SessionId,
        [property: JsonProperty("securityToken")] string SecurityToken,
        [property: JsonProperty("playerId")] string PlayerId,
        [property: JsonProperty("providerGameId")] string ProviderGameId,
        [property: JsonProperty("playMode")] int PlayMode,
        [property: JsonProperty("roundId")] string RoundId,
        [property: JsonProperty("transactionId")] string TransactionId,
        [property: JsonProperty("currency")] string Currency,
        [property: JsonProperty("amount")] long Amount,
        [property: JsonProperty("closeRound")] bool CloseRound)
    : IAnakatechChargeRequest,
      IRequest<IAnakatechResult<AnakatechRollbackResponse>>
{
    public sealed class Handler : IRequestHandler<AnakatechRollbackRequest, IAnakatechResult<AnakatechRollbackResponse>>
    {
        private readonly IWalletService _walletService;

        public Handler(IWalletService walletService)
        {
            _walletService = walletService;
        }

        public async Task<IAnakatechResult<AnakatechRollbackResponse>> Handle(
            AnakatechRollbackRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _walletService.RollbackAsync(
                request.SessionId,
                roundId: request.RoundId,
                transactionId: request.TransactionId,
                amount: MoneyHelper.ConvertFromCents(request.Amount),
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
            {
                return walletResult.ToAnakatechFailureResult<AnakatechRollbackResponse>();
            }

            var data = walletResult.Data;

            var response = new AnakatechRollbackResponse(
                true,
                data.Transaction.Id,
                MoneyHelper.ConvertToCents(data.Balance),
                data.Currency);

            return walletResult.ToAnakatechResult(response);
        }
    }

    public sealed class AnakatechRollbackRequestValidator : AbstractValidator<AnakatechRollbackRequest>
    {
        public AnakatechRollbackRequestValidator()
        {
            RuleFor(x => x.Secret)
               .NotNull()
               .WithMessage("Secret is required.")
               .Length(1, 24)
               .WithMessage("Secret length should be between 1 and 24 characters.");

            RuleFor(x => x.SessionId)
               .NotNull()
               .WithMessage("SessionId is required.")
               .Length(1, 256)
               .WithMessage("SessionId length should be between 1 and 256 characters.");

            RuleFor(x => x.SecurityToken)
               .NotNull()
               .WithMessage("SecurityToken is required.")
               .Length(1, 256)
               .WithMessage("SecurityToken length should be between 1 and 256 characters.");

            RuleFor(x => x.PlayerId)
               .NotNull()
               .WithMessage("PlayerId is required.")
               .Length(1, 32)
               .WithMessage("PlayerId length should be between 1 and 32 characters.");

            RuleFor(x => x.ProviderGameId)
               .NotNull()
               .WithMessage("ProviderGameId is required.")
               .Length(1, 256)
               .WithMessage("ProviderGameId length should be between 1 and 256 characters.");

            RuleFor(x => x.PlayMode)
               .Must(x => x is (int)AnakatechPlayMode.RealMoney or (int)AnakatechPlayMode.Anonymous);

            RuleFor(x => x.RoundId)
               .NotNull()
               .WithMessage("RoundId is required.")
               .Length(1, 256)
               .WithMessage("RoundId length should be between 1 and 256 characters.");

            RuleFor(x => x.TransactionId)
               .NotNull()
               .WithMessage("TransactionId is required.")
               .Length(1, 36)
               .WithMessage("TransactionId length should be between 1 and 36 characters.");

            RuleFor(x => x.Currency)
               .NotNull()
               .WithMessage("Currency is required.")
               .Length(3)
               .WithMessage("Currency length should be exactly 3 characters.");

            RuleFor(x => x.Amount)
               .NotEmpty()
               .Must(x => x.ToString().Length <= 32);

            RuleFor(x => x.CloseRound)
               .NotNull()
               .WithMessage("CloseRound is required.");
        }
    }
}