using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Base;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Responses;
using Platipus.Wallet.Api.Application.Results.EmaraPlay;
using Platipus.Wallet.Api.Application.Results.EmaraPlay.WithData;
using Platipus.Wallet.Api.Application.Services.Wallet;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay;

using Application.Results.ResultToResultMappers;
using FluentValidation;
using Services.EmaraPlayGameApi;
using Services.EmaraPlayGameApi.Requests;

public sealed record EmaraPlayGetRoundDetailsRequest(
    string Environment, 
    EmaraplayGetRoundDetailsGameApiRequest ApiRequest,
    string? Token = null) : 
        IEmaraPlayBaseRequest, IRequest<IEmaraPlayResult<EmaraPlayGetRoundDetailsResponse>>
{
    public sealed class Handler :
        IRequestHandler<EmaraPlayGetRoundDetailsRequest, IEmaraPlayResult<EmaraPlayGetRoundDetailsResponse>>
    {
        private readonly IEmaraPlayGameApiClient _apiClient;
        private readonly IWalletService _walletService;

        public Handler(
            IEmaraPlayGameApiClient apiClient, 
            IWalletService walletService)
        {
            _apiClient = apiClient;
            _walletService = walletService;
        }

        public async Task<IEmaraPlayResult<EmaraPlayGetRoundDetailsResponse>> Handle(
            EmaraPlayGetRoundDetailsRequest request, CancellationToken cancellationToken)
        {
            var walletResponse = await _walletService.GetEnvironmentAsync(
                request.Environment, cancellationToken);
            
            var clientResponse = await _apiClient.GetRoundDetailsAsync(
                walletResponse.Data.BaseUrl, request.ApiRequest, cancellationToken);
            
            if (clientResponse.IsFailure)
                return clientResponse.ToEmaraPlayResult<EmaraPlayGetRoundDetailsResponse>();
            
            var response = new EmaraPlayGetRoundDetailsResponse(clientResponse.Data.Data.Result);
            return EmaraPlayResultFactory.Success(response);
        }
    }
    
    internal sealed class EmaraPlayGetRoundDetailsRequestValidator : AbstractValidator<EmaraPlayGetRoundDetailsRequest>
    {
        public EmaraPlayGetRoundDetailsRequestValidator()
        {
            RuleFor(x => x.Environment)
                .NotEmpty()
                .WithMessage("Environment is required.");

            RuleFor(x => x.ApiRequest)
                .SetValidator(new EmaraplayGetRoundDetailsGameApiRequestValidator());
        }
    }

    private sealed class EmaraplayGetRoundDetailsGameApiRequestValidator : AbstractValidator<EmaraplayGetRoundDetailsGameApiRequest>
    {
        public EmaraplayGetRoundDetailsGameApiRequestValidator()
        {
            RuleFor(x => x.Bet)
                .NotEmpty()
                .WithMessage("Bet is required.");
        }
    }
}