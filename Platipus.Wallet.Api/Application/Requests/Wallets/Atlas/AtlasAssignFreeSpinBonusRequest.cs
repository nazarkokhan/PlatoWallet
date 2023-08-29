namespace Platipus.Wallet.Api.Application.Requests.Wallets.Atlas;

using System.Text.Json.Serialization;
using Base;
using FluentValidation;
using Results.Atlas;
using Results.ResultToResultMappers;
using Services.AtlasGameApi;
using Services.AtlasGameApi.Requests;
using Services.Wallet;

public sealed record AtlasAssignFreeSpinBonusRequest(
        [property: JsonPropertyName("environment")]string Environment,
        [property: JsonPropertyName("apiRequest")]AtlasAssignFreeSpinBonusGameApiRequest ApiRequest,
    [property: JsonPropertyName("token")]string? Token = null) 
        : IAtlasRequest, IRequest<IAtlasResult>
{
    public sealed class Handler 
        : IRequestHandler<AtlasAssignFreeSpinBonusRequest, IAtlasResult>
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
            AtlasAssignFreeSpinBonusRequest request, 
            CancellationToken cancellationToken)
        {
            var walletResponse = await _walletService.GetEnvironmentAsync(
                request.Environment, cancellationToken);
            
            if (walletResponse.IsFailure)
            {
                return walletResponse.ToAtlasFailureResult<AtlasErrorResponse>();
            }
            
            var clientResponse = await _atlasGameApiClient.AssignFreeSpinBonusAsync(
                walletResponse.Data?.BaseUrl!, request.ApiRequest, request.Token!,
                cancellationToken);

            return clientResponse.IsFailure 
                ? clientResponse.ToAtlasFailureResult<AtlasErrorResponse>() 
                : clientResponse.ToAtlasResult();
        }
    }
    
    public sealed class AtlasAssignFreeSpinBonusRequestValidator 
        : AbstractValidator<AtlasAssignFreeSpinBonusRequest>
    {
        public AtlasAssignFreeSpinBonusRequestValidator()
        {
            RuleFor(x => x.Environment).NotEmpty();

            RuleFor(x => x.ApiRequest).SetValidator(
                new AtlasAssignFreeSpinBonusGameApiRequestValidator());
        }
    }

    private sealed class AtlasAssignFreeSpinBonusGameApiRequestValidator 
        : AbstractValidator<AtlasAssignFreeSpinBonusGameApiRequest>
    {
        public AtlasAssignFreeSpinBonusGameApiRequestValidator()
        {
            RuleFor(x => x.BonusId)
                .NotEmpty();
            
            When(x => x.CasinoId is not null, () =>
            {
                RuleFor(x => x.CasinoId).NotEmpty();
            });

            RuleFor(x => x.BonusInstanceId).NotEmpty();
            
            RuleFor(x => x.ClientId).NotEmpty();

            RuleFor(x => x.Currency).NotEmpty()
                .Must(BeAValidCurrency)
                .WithMessage("Invalid currency format");
        }

        private static bool BeAValidCurrency(string currency)
        {
            // Assuming the currency string is an ISO 4217 code, we need to check its length.
            // ISO 4217 currency codes are three letters long.
            return currency.Length is 3;
        }
    }
}