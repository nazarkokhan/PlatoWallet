using Microsoft.EntityFrameworkCore;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Base;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Responses;
using Platipus.Wallet.Api.Application.Results.EmaraPlay;
using Platipus.Wallet.Api.Application.Results.EmaraPlay.WithData;
using Platipus.Wallet.Api.Application.Services.EmaraPlayGamesApi;
using Platipus.Wallet.Domain.Entities;
using Platipus.Wallet.Infrastructure.Persistence;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay;

public sealed record EmaraPlayAwardRequest(
    string Environment, string User, string Count, string EndDate, 
    string Currency, List<string>? Games = null, string? Code = null,
    string? MinBet = "0", string? MaxBet = null, string? StartDate = null, 
    string? Operator = null, string? Token = null) : 
    IEmaraPlayBaseRequest, IRequest<IEmaraPlayResult<EmaraPlayAwardResponse>>
{
    public sealed class Handler :
        IRequestHandler<EmaraPlayAwardRequest, IEmaraPlayResult<EmaraPlayAwardResponse>>
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

        public async Task<IEmaraPlayResult<EmaraPlayAwardResponse>> Handle(
            EmaraPlayAwardRequest request, CancellationToken cancellationToken)
        {
            var environment = await _walletDbContext.Set<GameEnvironment>()
                .Where(e => e.Id == request.Environment)
                .FirstOrDefaultAsync(cancellationToken);

            if (environment is null)
                return EmaraPlayResultFactory.Failure<EmaraPlayAwardResponse>(
                    EmaraPlayErrorCode.InternalServerError);
            
            var clientResponse = await _apiClient.GetAwardAsync(
                environment.BaseUrl, request, cancellationToken);
            
            var response = new EmaraPlayAwardResponse(
                0, "Success", clientResponse.Data.Data.Result);
            return EmaraPlayResultFactory.Success(response);
        }
    }
}