namespace Platipus.Wallet.Api.Application.Services.UranusGamesApi.External;

using System.Text.Json.Serialization;
using Application.Requests.Wallets.Uranus.Base;
using FluentValidation;
using Requests;
using Wallet;

public sealed record UranusCreateCampaignRequest(
        [property: JsonPropertyName("environment")] string Environment,
        [property: JsonPropertyName("apiRequest")] UranusCreateCampaignGameApiRequest ApiRequest)
    : IRequest<IResult<UranusSuccessResponse<string[]>>>
{
    public sealed class Handler : IRequestHandler<UranusCreateCampaignRequest, IResult<UranusSuccessResponse<string[]>>>
    {
        private readonly IWalletService _walletService;
        private readonly IUranusGameApiClient _uranusGameApiClient;

        public Handler(
            IWalletService walletService,
            IUranusGameApiClient uranusGameApiClient)
        {
            _walletService = walletService;
            _uranusGameApiClient = uranusGameApiClient;
        }

        public async Task<IResult<UranusSuccessResponse<string[]>>> Handle(
            UranusCreateCampaignRequest request,
            CancellationToken cancellationToken)
        {
            var walletResponse = await _walletService.GetEnvironmentAsync(
                request.Environment,
                cancellationToken);

            var clientResponse = await _uranusGameApiClient.CreateCampaignAsync(
                walletResponse.Data.BaseUrl,
                request.ApiRequest,
                cancellationToken);

            if (clientResponse.IsFailure)
                return ResultFactory.Failure<UranusSuccessResponse<string[]>>(
                    ErrorCode.GameServerApiError,
                    clientResponse.Exception);

            var response = new UranusSuccessResponse<string[]>(Array.Empty<string>());

            return ResultFactory.Success(response);
        }
    }

    public sealed class Validator : AbstractValidator<UranusCreateCampaignRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Environment)
               .NotEmpty();

            RuleFor(x => x.ApiRequest.Currency)
               .NotEmpty()
               .WithMessage("Currency cannot be empty or null.");

            RuleFor(x => x.ApiRequest.PlayerId)
               .NotEmpty()
               .WithMessage("PlayerId cannot be empty or null.");

            RuleFor(x => x.ApiRequest.PlayerCampaignId)
               .NotEmpty()
               .WithMessage("PlayerCampaignId cannot be empty or null.");

            RuleFor(x => x.ApiRequest.CampaignEndTime)
               .NotEmpty()
               .WithMessage("CampaignEndTime cannot be empty or null.");

            RuleFor(x => x.ApiRequest.SpinsCount)
               .GreaterThan(0)
               .WithMessage("SpinsCount must be greater than 0.");

            RuleFor(x => x.ApiRequest.GameIds)
               .NotEmpty()
               .WithMessage("GameIds cannot be empty or null.")
               .Must(gameIds => gameIds.Count > 0)
               .WithMessage("GameIds must contain at least one item.");
        }
    }
}