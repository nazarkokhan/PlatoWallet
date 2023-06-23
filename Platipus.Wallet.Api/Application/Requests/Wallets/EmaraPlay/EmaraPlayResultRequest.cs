using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Base;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Models;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Responses;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Results;
using Platipus.Wallet.Api.Application.Results.EmaraPlay;
using Platipus.Wallet.Api.Application.Results.EmaraPlay.WithData;
using Platipus.Wallet.Api.Application.Services.Wallet;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay;

using System.Text.Json.Serialization;
using Application.Results.ResultToResultMappers;
using FluentValidation;

public sealed record EmaraPlayResultRequest(
        string User, 
        string Game, 
        string Bet, 
        decimal Amount, 
        string Transaction,
        string Provider, 
        string Token, 
        [property: JsonPropertyName("closeRound")]bool CloseRound, 
        [property: JsonPropertyName("betBonusAmount")]string? BetBonusAmount = null, 
        List<Jackpot>? Jackpots = null, 
        [property: JsonPropertyName("bonusCode")]string? BonusCode = null, 
        [property: JsonPropertyName("bonusAmount")]string? BonusAmount = null, 
        List<Detail>? Details = null, 
        string? Spins = null) 
    : IEmaraPlayBaseRequest, IRequest<IEmaraPlayResult<EmaraPlayResultResponse>>
{
    public sealed class Handler 
        : IRequestHandler<EmaraPlayResultRequest, IEmaraPlayResult<EmaraPlayResultResponse>>
    {
        private readonly IWalletService _walletService;

        public Handler(IWalletService walletService)
        {
            _walletService = walletService;
        }

        public async Task<IEmaraPlayResult<EmaraPlayResultResponse>> Handle(
            EmaraPlayResultRequest request, CancellationToken cancellationToken)
        {
            var walletResult = await _walletService.WinAsync(
                request.Token,
                request.Bet,
                request.Transaction,
                request.Amount, 
                request.CloseRound,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToEmaraPlayResult<EmaraPlayResultResponse>();

            var walletData = walletResult.Data;
            var winResult = new EmaraplayWinResult(walletData.Currency,
                walletData.Balance,
                walletData.Transaction.Id, walletData.Transaction.InternalId);

            var response = new EmaraPlayResultResponse(winResult);
            return EmaraPlayResultFactory.Success(response);

        }
    }
    

    public class EmaraPlayResultRequestValidator : AbstractValidator<EmaraPlayResultRequest>
    {
        public EmaraPlayResultRequestValidator()
        {
            RuleFor(x => x.User)
                .NotEmpty()
                .WithMessage("User is required.");

            RuleFor(x => x.Game)
                .NotEmpty()
                .WithMessage("Game is required.");

            RuleFor(x => x.Bet)
                .NotEmpty()
                .WithMessage("Bet is required.");

            RuleFor(x => x.Amount)
                .NotEmpty()
                .WithMessage("Amount is required.")
                .GreaterThan(0)
                .WithMessage("Amount must be greater than 0.");

            RuleFor(x => x.Transaction)
                .NotEmpty()
                .WithMessage("Transaction is required.");

            RuleFor(x => x.Provider)
                .NotEmpty()
                .WithMessage("Provider is required.");

            RuleFor(x => x.Token)
                .NotEmpty()
                .WithMessage("Token is required.");

            RuleFor(x => x.CloseRound)
                .NotNull()
                .WithMessage("CloseRound is required.");

            RuleFor(x => x.Jackpots)
                .SetValidator(new InlineValidator<List<Jackpot>>
                {
                    v => v.RuleFor(x => x)
                        .Must(list => list is null || list.All(i => !string.IsNullOrEmpty(i.Id) && i.Amount > 0))
                        .WithMessage("All Jackpots must have a non-empty Id and Amount greater than 0.")
                }!);

            RuleFor(x => x.Details)
                .SetValidator(new InlineValidator<List<Detail>>
                {
                    v => v.RuleFor(x => x)
                        .Must(list => list is null || list.All(i => !string.IsNullOrEmpty(i.Type) && 
                                                                    !string.IsNullOrEmpty(i.Value)))
                        .WithMessage("All Details must have a non-empty Type and Value.")
                }!);

            When(x => !string.IsNullOrEmpty(x.BetBonusAmount), () =>
            {
                RuleFor(x => x.BetBonusAmount)
                    .Must(x => decimal.TryParse(x, out _))
                    .WithMessage("BetBonusAmount must be a valid decimal.");
            });

            When(x => !string.IsNullOrEmpty(x.BonusAmount), () =>
            {
                RuleFor(x => x.BonusAmount)
                    .Must(x => decimal.TryParse(x, out _))
                    .WithMessage("BonusAmount must be a valid decimal.");
            });

            When(x => !string.IsNullOrEmpty(x.Spins), () =>
            {
                RuleFor(x => x.Spins)
                    .Must(x => int.TryParse(x, out _))
                    .WithMessage("Spins must be a valid integer.");
            });
        }
    }
}