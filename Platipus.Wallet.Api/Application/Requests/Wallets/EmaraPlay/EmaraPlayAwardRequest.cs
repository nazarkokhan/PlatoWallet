using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Base;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Responses;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Results;
using Platipus.Wallet.Api.Application.Results.EmaraPlay;
using Platipus.Wallet.Api.Application.Results.EmaraPlay.WithData;
using Platipus.Wallet.Api.Application.Services.Wallet;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay;

using FluentValidation;
using Services.EmaraPlayGameApi;
using Services.EmaraPlayGameApi.Requests;

public sealed record EmaraPlayAwardRequest(
    string Environment, 
    EmaraplayAwardGameApiRequest ApiRequest, 
    string? Token = null) : IEmaraPlayBaseRequest, IRequest<IEmaraPlayResult<EmaraPlayAwardResponse>>
{
    public sealed class Handler :
        IRequestHandler<EmaraPlayAwardRequest, IEmaraPlayResult<EmaraPlayAwardResponse>>
    {
        private readonly IWalletService _walletService;
        private readonly IEmaraPlayGameApiClient _apiClient;

        public Handler(
            IEmaraPlayGameApiClient apiClient, IWalletService walletService)
        {
            _apiClient = apiClient;
            _walletService = walletService;
        }

        public async Task<IEmaraPlayResult<EmaraPlayAwardResponse>> Handle(
            EmaraPlayAwardRequest request, CancellationToken cancellationToken)
        {
            var walletResponse = await _walletService.GetEnvironmentAsync(
                request.Environment, cancellationToken);
            
            var clientResponse = await _apiClient.GetAwardAsync(
                walletResponse.Data.BaseUrl, request.ApiRequest, cancellationToken);

            if (clientResponse.IsFailure)
                return EmaraPlayResultFactory.Failure<EmaraPlayAwardResponse>(
                    EmaraPlayErrorCode.InternalServerError);
            
            var data = clientResponse.Data.Data.Result;
            var response = new EmaraPlayAwardResponse(
                new EmaraplayAwardResult(data.Ref));
            return EmaraPlayResultFactory.Success(response);
        }
    }
    
    internal sealed class Validator : AbstractValidator<EmaraPlayAwardRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Environment)
                .NotEmpty()
                .WithMessage("Environment is required.");

            RuleFor(x => x.ApiRequest)
                .SetValidator(new EmaraplayAwardGameApiRequestValidator());
        }
    }
    private sealed class EmaraplayAwardGameApiRequestValidator : AbstractValidator<EmaraplayAwardGameApiRequest>
    {
        public EmaraplayAwardGameApiRequestValidator()
        {
            RuleFor(x => x.User)
                .NotEmpty()
                .WithMessage("User is required.");

            RuleFor(x => x.Count)
                .NotEmpty()
                .WithMessage("Count is required.")
                .Must(x => int.TryParse(x, out _))
                .WithMessage("Count must be a valid integer.");

            RuleFor(x => x.EndDate)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("EndDate is required.")
                .Must(x => long.TryParse(x, out _))
                .WithMessage("EndDate must be a valid long integer representing milliseconds since Unix epoch.");

            RuleFor(x => x.Currency)
                .NotEmpty()
                .WithMessage("Currency is required.");

            RuleFor(x => x.MinBet)
                .Must(x => decimal.TryParse(x, out _))
                .WithMessage("MinBet must be a valid decimal or null.");

            RuleFor(x => x.MaxBet)
                .Must(x => string.IsNullOrEmpty(x) || decimal.TryParse(x, out _))
                .WithMessage("MaxBet must be a valid decimal or null.");

            RuleFor(x => x.StartDate)
                .Must(x => string.IsNullOrEmpty(x) || long.TryParse(x, out var _))
                .WithMessage("StartDate must be a valid long integer representing milliseconds since Unix epoch or null.");
        }
    }
}