namespace Platipus.Wallet.Api.Application.Requests.Wallets.Parimatch;

using Base;
using Domain.Entities;
using Helpers.Common;
using Infrastructure.Persistence;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Responses;
using Results.Parimatch;
using Results.Parimatch.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;

[PublicAPI]
public record ParimatchPromoWinRequest(
        string Cid,
        string PlayerId,
        string TxId,
        long Amount)
    : IRequest<IParimatchResult<ParimatchPromoWinResponse>>, IParimatchPlayerIdRequest
{
    public sealed class Handler : IRequestHandler<ParimatchPromoWinRequest, IParimatchResult<ParimatchPromoWinResponse>>
    {
        private readonly IWalletService _walletService;
        private readonly WalletDbContext _dbContext;

        public Handler(IWalletService walletService, WalletDbContext dbContext)
        {
            _walletService = walletService;
            _dbContext = dbContext;
        }

        public async Task<IParimatchResult<ParimatchPromoWinResponse>> Handle(
            ParimatchPromoWinRequest request,
            CancellationToken cancellationToken)
        {
            var award = await _dbContext.Set<Award>()
               .Where(
                    a => a.User.Username == request.PlayerId
                      && a.User.CasinoId == request.Cid
                      && a.ValidUntil > DateTime.UtcNow
                      && !a.AwardRound.Any())
               .OrderBy(a => a.CreatedDate)
               .FirstOrDefaultAsync(cancellationToken);
            if (award is null || award.AwardRound.Any())
                return ParimatchResultFactory.Failure<ParimatchPromoWinResponse>(ParimatchErrorCode.InvalidCasinoLogic);

            var walletResult = await _walletService.AwardAsync(
                request.PlayerId,
                request.TxId,
                request.TxId,
                MoneyHelper.ConvertFromCents(request.Amount),
                award.Id,
                searchByUsername: true,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToParimatchFailureResult<ParimatchPromoWinResponse>();
            var data = walletResult.Data;

            var response = new ParimatchPromoWinResponse(
                request.TxId,
                DateTimeOffset.UtcNow, //TODO get from data
                MoneyHelper.ConvertToCents(data.Balance));

            return ParimatchResultFactory.Success(response);
        }
    }
}