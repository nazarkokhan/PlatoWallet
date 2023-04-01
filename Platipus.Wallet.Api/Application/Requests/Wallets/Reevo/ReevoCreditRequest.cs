namespace Platipus.Wallet.Api.Application.Requests.Wallets.Reevo;

using Base;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.Reevo;
using Results.Reevo.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;

public record ReevoCreditRequest(
    string CallerId,
    string CallerPassword,
    string Action,
    int RemoteId,
    string Username,
    string SessionId,
    double Amount,
    string GameIdHash,
    string TransactionId,
    string RoundId,
    int GameplayFinal,
    int IsFreeRoundWin,
    int? FreeroundSpinsRemaining,
    int? FreeroundCompleted,
    string? FreeRoundId,
    int IsJackpotWin,
    double? JackpotWinInAmount,
    string GameSessionId,
    string Key) : IRequest<IReevoResult<ReevoSuccessResponse>>, IReevoRequest
{
    public class Handler : IRequestHandler<ReevoCreditRequest, IReevoResult<ReevoSuccessResponse>>
    {
        private readonly IWalletService _wallet;
        private readonly WalletDbContext _context;

        public Handler(IWalletService wallet, WalletDbContext context)
        {
            _wallet = wallet;
            _context = context;
        }

        public async Task<IReevoResult<ReevoSuccessResponse>> Handle(
            ReevoCreditRequest request,
            CancellationToken cancellationToken)
        {
            if (request.IsFreeRoundWin is 1)
            {
                var award = await _context.Set<Award>()
                    .Where(a => a.Id == request.FreeRoundId)
                    .FirstOrDefaultAsync(cancellationToken);

                if (award is null || award.ValidUntil < DateTime.UtcNow)
                    return ReevoResultFactory.Failure<ReevoSuccessResponse>(ReevoErrorCode.InternalError);
            }

            var walletResult = await _wallet.WinAsync(
                request.SessionId,
                request.RoundId,
                request.TransactionId,
                (decimal)request.Amount,
                roundFinished: request.GameplayFinal is 1,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToReevoResult<ReevoSuccessResponse>();
            var data = walletResult.Data;

            var response = new ReevoSuccessResponse(data.Balance);

            return ReevoResultFactory.Success(response);
        }
    }
}