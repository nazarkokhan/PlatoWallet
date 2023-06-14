using Microsoft.EntityFrameworkCore;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Base;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Responses;
using Platipus.Wallet.Api.Application.Results.EmaraPlay;
using Platipus.Wallet.Api.Application.Results.EmaraPlay.WithData;
using Platipus.Wallet.Api.Application.Services.EmaraPlayGamesApi;
using Platipus.Wallet.Domain.Entities;
using Platipus.Wallet.Infrastructure.Persistence;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay;

public sealed record EmaraPlayGetRoundDetailsRequest(
    string Environment, string Bet, 
    string? User = null, string? Game = null,
    string? Operator = null, string? Currency = null,
    string? Token = null) : 
        IEmaraPlayBaseRequest, IRequest<IEmaraPlayResult<EmaraPlayGetRoundDetailsResponse>>
{
    public sealed class Handler :
        IRequestHandler<EmaraPlayGetRoundDetailsRequest, IEmaraPlayResult<EmaraPlayGetRoundDetailsResponse>>
    {
        private readonly IEmaraPlayGameApiClient _apiClient;
        private readonly WalletDbContext _walletDbContext;

        public Handler(IEmaraPlayGameApiClient apiClient, WalletDbContext walletDbContext)
        {
            _apiClient = apiClient;
            _walletDbContext = walletDbContext;
        }

        public async Task<IEmaraPlayResult<EmaraPlayGetRoundDetailsResponse>> Handle(
            EmaraPlayGetRoundDetailsRequest request, CancellationToken cancellationToken)
        {
            var environment = await _walletDbContext.Set<GameEnvironment>()
                .Where(e => e.Id == request.Environment)
                .FirstOrDefaultAsync(cancellationToken);

            if (environment is null)
                return EmaraPlayResultFactory.Failure<EmaraPlayGetRoundDetailsResponse>(
                    EmaraPlayErrorCode.InternalServerError);
            
            var clientResponse = await _apiClient.GetRoundDetailsAsync(
                environment.BaseUrl, request, cancellationToken);
            
            var response = new EmaraPlayGetRoundDetailsResponse(
                0, "Success", clientResponse.Data.Data.Result);
            return EmaraPlayResultFactory.Success(response);
        }
    }
}