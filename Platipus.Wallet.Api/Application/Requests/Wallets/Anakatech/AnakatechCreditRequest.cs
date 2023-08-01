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

public sealed record AnakatechCreditRequest(
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
        [property: JsonProperty("closeRound")] bool CloseRound,
        [property: JsonProperty("secondaryRoundId")] string? SecondaryRoundId = null)
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

            if (walletResult.IsFailure || walletResult.Data is null)
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

    //TODO how do you convert it to acceptable response that replicates real wallet behaviour?
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
               .NotEmpty()
               .Must(x => x.ToString().Length <= 32)
               .Must(x => x >= 0);

            RuleFor(x => x.SecondaryRoundId)
               .Length(1, 256)
               .Unless(x => x.SecondaryRoundId is null);
        }
    }
}