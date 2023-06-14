using Microsoft.EntityFrameworkCore;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Base;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Responses;
using Platipus.Wallet.Api.Application.Results.EmaraPlay;
using Platipus.Wallet.Api.Application.Results.EmaraPlay.WithData;
using Platipus.Wallet.Api.Application.Services.EmaraPlayGamesApi;
using Platipus.Wallet.Domain.Entities;
using Platipus.Wallet.Infrastructure.Persistence;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay;

public sealed record EmaraPlayGetLauncherUrlRequest(
    string Environment,
    string Operator, string Token, string Game, 
    string Mode, string Lang, string Channel,
    string Jurisdiction, string Currency, string Ip,
    string? User, string? Lobby = null, 
    string? Cashier = null) : IEmaraPlayBaseRequest, IRequest<IEmaraPlayResult<EmaraPlayGetLauncherUrlResponse>>
{
    public sealed class Handler : 
        IRequestHandler<EmaraPlayGetLauncherUrlRequest, IEmaraPlayResult<EmaraPlayGetLauncherUrlResponse>>
    {
        private readonly IEmaraPlayGameApiClient _gameApiClient;
        private readonly WalletDbContext _walletDbContext;

        public Handler(IEmaraPlayGameApiClient gameApiClient, WalletDbContext walletDbContext)
        {
            _gameApiClient = gameApiClient;
            _walletDbContext = walletDbContext;
        }

        public async Task<IEmaraPlayResult<EmaraPlayGetLauncherUrlResponse>> Handle(
            EmaraPlayGetLauncherUrlRequest urlRequest, CancellationToken cancellationToken)
        {
            var environment = await _walletDbContext.Set<GameEnvironment>()
                .Where(e => e.Id == urlRequest.Environment)
                .FirstOrDefaultAsync(cancellationToken);

            if (environment is null)
                return EmaraPlayResultFactory.Failure<EmaraPlayGetLauncherUrlResponse>(
                    EmaraPlayErrorCode.InternalServerError);
            var clientResponse = await _gameApiClient.GetLauncherUrlAsync(
                environment.BaseUrl, urlRequest, cancellationToken);
            
            var response = new EmaraPlayGetLauncherUrlResponse(
                0, "Success", clientResponse.Data.Data.Result);
            return EmaraPlayResultFactory.Success(response);
        }
    }
}