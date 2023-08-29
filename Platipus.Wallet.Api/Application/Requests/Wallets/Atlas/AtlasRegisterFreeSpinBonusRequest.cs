namespace Platipus.Wallet.Api.Application.Requests.Wallets.Atlas;

using System.Text.Json.Serialization;
using Base;
using FluentValidation;
using Results.Atlas;
using Results.ResultToResultMappers;
using Services.AtlasGameApi;
using Services.AtlasGameApi.Requests;
using Services.Wallet;

public sealed record AtlasRegisterFreeSpinBonusRequest(
        [property: JsonPropertyName("environment")] 
        string Environment,
        [property: JsonPropertyName("apiRequest")]
        AtlasRegisterFreeSpinBonusGameApiRequest ApiRequest,
        [property: JsonPropertyName("token")] string? Token = null)
    : IAtlasRequest, IRequest<IAtlasResult>
{
    public sealed class Handler : 
        IRequestHandler<AtlasRegisterFreeSpinBonusRequest, IAtlasResult>
    {
        private readonly IWalletService _walletService;
        private readonly IAtlasGameApiClient _atlasGameApiClient;

        public Handler(
            IWalletService walletService, 
            IAtlasGameApiClient atlasGameApiClient)
        {
            _walletService = walletService;
            _atlasGameApiClient = atlasGameApiClient;
        }

        public async Task<IAtlasResult> Handle(
            AtlasRegisterFreeSpinBonusRequest request, 
            CancellationToken cancellationToken)
        {
            var walletResponse = await _walletService.GetEnvironmentAsync(
                request.Environment, cancellationToken);
            if (walletResponse.IsFailure)
            {
                return walletResponse.ToAtlasFailureResult<AtlasErrorResponse>();
            }
            
            var clientResponse = await _atlasGameApiClient.RegisterFreeSpinBonusAsync(
                walletResponse.Data?.BaseUrl!, request.ApiRequest, request.Token!,
                cancellationToken);

            return clientResponse.IsFailure 
                ? clientResponse.ToAtlasFailureResult<AtlasErrorResponse>() 
                : clientResponse.ToAtlasResult();
        }
    }

    public sealed class Validator : AbstractValidator<AtlasRegisterFreeSpinBonusRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Environment).NotEmpty();

            RuleFor(x => x.ApiRequest).SetValidator(
                new AtlasRegisterFreeSpinBonusGameApiRequestValidator());
        }
    }

    private sealed class AtlasRegisterFreeSpinBonusGameApiRequestValidator : AbstractValidator<AtlasRegisterFreeSpinBonusGameApiRequest>
    {
        public AtlasRegisterFreeSpinBonusGameApiRequestValidator()
        {
            RuleFor(x => x.GameId).NotEmpty();

            RuleFor(x => x.BonusId).NotEmpty();
            
            RuleFor(x => x.CasinoId)
                .NotEmpty()
                .When(x => x.CasinoId is not null);

            RuleFor(x => x.SpinsCount).GreaterThan(0);

            RuleForEach(x => x.BetValues)
                .NotEmpty() 
                .Must(HaveValidBetValues)
                .WithMessage("Invalid BetValues format");

            RuleFor(x => x.StartDate)
            .NotEmpty()
            .Must(BeAValidDate)
            .WithMessage("Invalid date format");

        RuleFor(x => x.ExpirationDate)
            .NotEmpty()
            .Must(BeAValidDate)
            .WithMessage("Invalid date format")
            .Must((request, expirationDate) => BeLaterThanStartDate(expirationDate, request.StartDate))
            .WithMessage("ExpirationDate must be later than StartDate")
            .Must(BeInTheFuture)
            .WithMessage("ExpirationDate must be in the future");
        }

        private static bool BeAValidDate(string date)
        {
            if (!long.TryParse(date, out var dateLong)) return false;
            DateTimeOffset.FromUnixTimeSeconds(dateLong);
            return true;

        }
        private static bool HaveValidBetValues(Dictionary<string, int> betValues)
        {
            if (betValues.Count is not 1) return false;
            var kvp = betValues.First();
            if (kvp.Key.Length is not 3) return false;
            return kvp.Value > 0;
        }
        
        private static bool BeLaterThanStartDate(string expirationDate, string startDate)
        {
            if (!long.TryParse(startDate, out var startUnixTime)) return false;
            if (!long.TryParse(expirationDate, out var expirationUnixTime)) return false;
            return expirationUnixTime > startUnixTime;
        }

        private static bool BeInTheFuture(string expirationDate)
        {
            if (!long.TryParse(expirationDate, out var expirationUnixTime)) return false;
            var currentUnixTime = DateTimeOffset.Now.ToUnixTimeSeconds();
            return expirationUnixTime > currentUnixTime;
        }
    }
}