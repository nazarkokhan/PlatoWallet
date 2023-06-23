namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay;

using System.Text.Json.Serialization;
using Application.Results.ResultToResultMappers;
using Base;
using FluentValidation;
using Models;
using Responses;
using Results;
using Platipus.Wallet.Api.Application.Results.EmaraPlay;
using Platipus.Wallet.Api.Application.Results.EmaraPlay.WithData;
using Platipus.Wallet.Api.Application.Services.Wallet;

public sealed record EmaraPlayBetRequest(
    string User,
    string Game,
    string Bet,
    string Provider,
    string Token,
    string Transaction,
    decimal Amount,
    [property: JsonPropertyName("bonusCode")]string? BonusCode = null,
    [property: JsonPropertyName("bonusAmount")]string? BonusAmount = null,
    List<Jackpot?>? Jackpots = null,
    string? Ip = null) : IEmaraPlayBaseRequest, IRequest<IEmaraPlayResult<EmaraPlayBetResponse>>
{
    public sealed class Handler : IRequestHandler<EmaraPlayBetRequest, IEmaraPlayResult<EmaraPlayBetResponse>>
    {
        private readonly IWalletService _walletService;

        public Handler(IWalletService walletService)
        {
            _walletService = walletService;
        }

        public async Task<IEmaraPlayResult<EmaraPlayBetResponse>> Handle(
            EmaraPlayBetRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _walletService.BetAsync(
                request.Token,
                request.Bet,
                transactionId: request.Transaction,
                request.Amount,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToEmaraPlayResult<EmaraPlayBetResponse>();
            
            var data = walletResult.Data;
            var response = new EmaraPlayBetResponse(
                new BetResult(data.Currency, data.Balance, data.Transaction.Id, data.Transaction.InternalId));

            return EmaraPlayResultFactory.Success(response);
        }
    }

    public sealed class Validator : AbstractValidator<EmaraPlayBetRequest>
    {
        public Validator()
        {
            RuleFor(x => x.User)
                .NotEmpty()
                .WithMessage("User is required.");

            RuleFor(x => x.Game)
                .NotEmpty()
                .WithMessage("Game is required.");

            RuleFor(x => x.Bet)
                .Length(5, 250)
                .WithMessage("Bet is required. Min length is 11 and max length is 250.");

            RuleFor(x => x.Provider)
                .NotEmpty()
                .WithMessage("Provider is required.");

            RuleFor(x => x.Token)
                .NotEmpty()
                .WithMessage("Token is required.");

            RuleFor(x => x.Transaction)
                .NotEmpty()
                .WithMessage("Transaction is required.");

            RuleFor(x => x.Amount)
                .InclusiveBetween(0, decimal.MaxValue)
                .WithMessage("Amount must be greater or equal to 0.");

            RuleForEach(x => x.Jackpots)
                .ChildRules(jackpot =>
                {
                    jackpot.RuleFor(x => x.Id)
                        .NotEmpty()
                        .WithMessage("Jackpot Id is required.");

                    jackpot.RuleFor(x => x.Amount)
                        .InclusiveBetween(0, decimal.MaxValue)
                        .WithMessage("Jackpot Amount must be greater or equal to 0.");
                });
            
            RuleFor(x => x.BonusCode)
                .Empty()
                .When(x => x.BonusAmount == null)
                .WithMessage("BonusCode and BonusAmount should be provided together");

            RuleFor(x => x.BonusAmount)
                .Empty()
                .When(x => x.BonusCode == null)
                .WithMessage("BonusCode and BonusAmount should be provided together");

        }
    }
}