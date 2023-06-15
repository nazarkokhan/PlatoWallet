using Microsoft.EntityFrameworkCore;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Base;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Responses;
using Platipus.Wallet.Api.Application.Results.EmaraPlay;
using Platipus.Wallet.Api.Application.Results.EmaraPlay.WithData;
using Platipus.Wallet.Api.Application.Services.EmaraPlayGamesApi;
using Platipus.Wallet.Domain.Entities;
using Platipus.Wallet.Infrastructure.Persistence;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay;

public sealed record EmaraPlayCancelRequest(
    string? Environment, string Ref, 
    string? Operator = null, string? Token = null) : 
    IEmaraPlayBaseRequest, IRequest<IEmaraPlayResult<EmaraPlayCancelResponse>>
{
    public sealed class Handler :
        IRequestHandler<EmaraPlayCancelRequest, IEmaraPlayResult<EmaraPlayCancelResponse>>
    {
        private readonly WalletDbContext _walletDbContext;
        private readonly IEmaraPlayGameApiClient _apiClient;

        public Handler(
            WalletDbContext walletDbContext, 
            IEmaraPlayGameApiClient apiClient)
        {
            _walletDbContext = walletDbContext;
            _apiClient = apiClient;
        }

        public async Task<IEmaraPlayResult<EmaraPlayCancelResponse>> Handle(
            EmaraPlayCancelRequest request, CancellationToken cancellationToken)
        {
            var environment = await _walletDbContext.Set<GameEnvironment>()
                .Where(e => e.Id == request.Environment)
                .FirstOrDefaultAsync(cancellationToken);

            if (environment is null)
                return EmaraPlayResultFactory.Failure<EmaraPlayCancelResponse>(
                    EmaraPlayErrorCode.InternalServerError);
            
            await _apiClient.CancelAsync(
                environment.BaseUrl, request, cancellationToken);
            
            var response = new EmaraPlayCancelResponse(
                0, "Success");
            return EmaraPlayResultFactory.Success(response);
        }
    }
}