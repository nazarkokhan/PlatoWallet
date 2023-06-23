namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay;

using Application.Results.ResultToResultMappers;
using Base;
using FluentValidation;
using Responses;
using Results;
using Platipus.Wallet.Api.Application.Results.EmaraPlay;
using Platipus.Wallet.Api.Application.Results.EmaraPlay.WithData;
using Platipus.Wallet.Api.Application.Services.Wallet;

public sealed record EmaraPlayBalanceRequest(
    string Provider,
    string Token,
    string User) : IEmaraPlayBaseRequest, IRequest<IEmaraPlayResult<EmaraPlayBalanceResponse>>
{
    public sealed class Handler : IRequestHandler<EmaraPlayBalanceRequest, IEmaraPlayResult<EmaraPlayBalanceResponse>>
    {
        private readonly IWalletService _walletService;
        private readonly ILogger<Handler> _logger;

        public Handler(
            ILogger<Handler> logger, 
            IWalletService walletService)
        {
            _logger = logger;
            _walletService = walletService;
        }

        public async Task<IEmaraPlayResult<EmaraPlayBalanceResponse>> Handle(
            EmaraPlayBalanceRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var walletResponse = await _walletService.GetBalanceAsync(
                    request.Token, cancellationToken: cancellationToken);

                if (walletResponse.IsFailure)
                    return walletResponse.ToEmaraPlayResult<EmaraPlayBalanceResponse>();

                var walletData = walletResponse.Data;
                var response = new EmaraPlayBalanceResponse(
                    new BalanceResult(walletData.Balance, walletData.Currency));

                return EmaraPlayResultFactory.Success(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Common wallet service GetBalance unknown exception");
                return EmaraPlayResultFactory.Failure<EmaraPlayBalanceResponse>(EmaraPlayErrorCode.InternalServerError,
                    e);
            }
        }
    }
    
    internal sealed class EmaraPlayBalanceRequestValidator : AbstractValidator<EmaraPlayBalanceRequest>
    {
        public EmaraPlayBalanceRequestValidator()
        {
            RuleFor(x => x.Provider)
                .NotEmpty()
                .WithMessage("Provider is required.");

            RuleFor(x => x.Token)
                .NotEmpty()
                .WithMessage("Token is required.");

            RuleFor(x => x.User)
                .NotEmpty()
                .WithMessage("User is required.");
        }
    }
}