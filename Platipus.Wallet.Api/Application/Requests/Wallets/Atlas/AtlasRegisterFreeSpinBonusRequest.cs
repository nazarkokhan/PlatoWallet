namespace Platipus.Wallet.Api.Application.Requests.Wallets.Atlas;

using Base;
using FluentValidation;
using Models;
using Results.Atlas;
using Results.ResultToResultMappers;
using Services.AtlasGamesApi;
using Services.AtlasGamesApi.Requests;
using Services.Wallet;

public sealed record AtlasRegisterFreeSpinBonusRequest(
    string Environment,
    AtlasRegisterFreeSpinBonusGameApiRequest ApiRequest,
    string? Token = null) : IAtlasRequest, IRequest<IAtlasResult>
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
            AtlasRegisterFreeSpinBonusRequest request, CancellationToken cancellationToken)
        {
            var walletResponse = await _walletService.GetEnvironmentAsync(
                request.Environment, cancellationToken);
            
            var clientResponse = await _atlasGameApiClient.RegisterFreeSpinBonusAsync(
                walletResponse.Data.BaseUrl, request.ApiRequest, request.Token!,
                cancellationToken);

            return clientResponse.ToAtlasResult();
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

            RuleFor(x => x.CasinoId).NotEmpty();

            RuleFor(x => x.SpinsCount).GreaterThan(0);

            RuleForEach(x => x.BetValues).SetValidator(
                new AtlasBetValueModelValidator());

            RuleFor(x => x.StartDate).NotEmpty()
                .Must(BeAValidDate).WithMessage("Invalid date format");

            RuleFor(x => x.ExpirationDate).NotEmpty()
                .Must(BeAValidDate).WithMessage("Invalid date format");
        }

        private static bool BeAValidDate(string date)
        {
            // Assuming the date string is a long in seconds, we need to try and parse it.
            if (!long.TryParse(date, out var dateLong)) return false;
            
            // Check if the date is a valid Unix timestamp
            DateTimeOffset.FromUnixTimeSeconds(dateLong);
            // Add your own validation logic here, if needed.
            return true;

        }
    }

    private sealed class AtlasBetValueModelValidator : AbstractValidator<AtlasBetValueModel>
    {
        public AtlasBetValueModelValidator()
        {
            RuleFor(x => x.Currency).NotEmpty().Must(x => 
                x.Length is 3);
            RuleFor(x => x.Value).GreaterThan(0);
        }
    }
}