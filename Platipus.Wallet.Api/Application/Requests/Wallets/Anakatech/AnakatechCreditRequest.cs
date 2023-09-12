namespace Platipus.Wallet.Api.Application.Requests.Wallets.Anakatech;

using System.Text.Json.Serialization;
using Base;
using Enums;
using FluentValidation;
using Helpers;
using Responses.Anakatech;
using Results.Anakatech.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;

public sealed record AnakatechCreditRequest(
        [property: JsonPropertyName("secret")] string Secret,
        [property: JsonPropertyName("sessionId")] string SessionId,
        [property: JsonPropertyName("securityToken")] string SecurityToken,
        [property: JsonPropertyName("playerId")] string PlayerId,
        [property: JsonPropertyName("providerGameId")] string ProviderGameId,
        [property: JsonPropertyName("playMode")] int PlayMode,
        [property: JsonPropertyName("roundId")] string RoundId,
        [property: JsonPropertyName("transactionId")] string TransactionId,
        [property: JsonPropertyName("currency")] string Currency,
        [property: JsonPropertyName("amount")] long Amount,
        [property: JsonPropertyName("closeRound")] bool CloseRound,
        [property: JsonPropertyName("secondaryRoundId")] string? SecondaryRoundId = null)
    : IAnakatechChargeRequest,
      IRequest<IAnakatechResult<AnakatechCreditResponse>>
{
    public sealed class Handler : IRequestHandler<AnakatechCreditRequest, IAnakatechResult<AnakatechCreditResponse>>
    {
        private readonly IWalletService _walletService;

        public Handler(IWalletService walletService)
        {
            _walletService = walletService;
        }

        public async Task<IAnakatechResult<AnakatechCreditResponse>> Handle(
            AnakatechCreditRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _walletService.WinAsync(
                request.SessionId,
                roundId: request.RoundId,
                transactionId: request.TransactionId,
                currency: request.Currency,
                amount: MoneyHelper.ConvertFromCents(request.Amount),
                roundFinished: request.CloseRound,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
            {
                return walletResult.ToAnakatechFailureResult<AnakatechCreditResponse>();
            }

            var data = walletResult.Data;

            var response = new AnakatechCreditResponse(
                true,
                data.Transaction.Id,
                MoneyHelper.ConvertToCents(data.Balance),
                data.Currency);

            return walletResult.ToAnakatechResult(response);
        }
    }
    
    public sealed class AnakatechCreditRequestValidator : AbstractValidator<AnakatechCreditRequest>
    {
        public AnakatechCreditRequestValidator()
        {
            RuleFor(x => x.Secret)
               .NotEmpty()
               .Length(1, 24);

            RuleFor(x => x.SessionId)
               .NotEmpty()
               .Length(1, 256);

            RuleFor(x => x.SecurityToken)
               .NotEmpty()
               .Length(1, 256);

            RuleFor(x => x.PlayerId)
               .NotEmpty()
               .Length(1, 32);

            RuleFor(x => x.ProviderGameId)
               .NotEmpty()
               .Length(1, 256);

            RuleFor(x => x.PlayMode)
               .Must(x => x is (int)AnakatechPlayMode.RealMoney or (int)AnakatechPlayMode.Anonymous);

            RuleFor(x => x.RoundId)
               .NotEmpty()
               .Length(1, 256);

            RuleFor(x => x.TransactionId)
               .NotEmpty()
               .Length(1, 36);

            RuleFor(x => x.Currency)
               .NotEmpty()
               .Length(3);

            RuleFor(x => x.Amount)
               .Must(x => x.ToString().Length <= 32);

            RuleFor(x => x.SecondaryRoundId)
               .Length(1, 256)
               .Unless(x => x.SecondaryRoundId is null);
        }
    }
}