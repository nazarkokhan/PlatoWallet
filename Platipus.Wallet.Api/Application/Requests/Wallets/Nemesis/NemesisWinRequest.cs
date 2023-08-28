namespace Platipus.Wallet.Api.Application.Requests.Wallets.Nemesis;

using System.Text.Json.Serialization;
using Base;
using Domain.Entities;
using Infrastructure.Persistence;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Responses;
using Results.Nemesis;
using Results.Nemesis.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;

[PublicAPI]
public sealed record NemesisWinRequest(
        [property: JsonPropertyName("session_token")] string SessionToken,
        [property: JsonPropertyName("player_id")] string PlayerId,
        [property: JsonPropertyName("game_id")] string GameId,
        [property: JsonPropertyName("transaction_id")] string TransactionId,
        [property: JsonPropertyName("round_id")] string RoundId,
        [property: JsonPropertyName("round_closed")] bool RoundClosed,
        [property: JsonPropertyName("amount")] decimal Amount,
        [property: JsonPropertyName("kind")] string? Kind,
        [property: JsonPropertyName("bonus_code")] string? BonusCode,
        [property: JsonPropertyName("currency")] string Currency)
    : INemesisRequest, IRequest<INemesisResult<NemesisBetWinRollbackResponse>>
{
    public sealed class Handler : IRequestHandler<NemesisWinRequest, INemesisResult<NemesisBetWinRollbackResponse>>
    {
        private readonly IWalletService _walletService;
        private readonly WalletDbContext _context;

        public Handler(IWalletService walletService, WalletDbContext context)
        {
            _walletService = walletService;
            _context = context;
        }

        public async Task<INemesisResult<NemesisBetWinRollbackResponse>> Handle(
            NemesisWinRequest request,
            CancellationToken cancellationToken)
        {
            var isAward = request.BonusCode is not null;
            if (isAward)
            {
                var award = await _context.Set<Award>()
                   .Where(a => a.Id == request.BonusCode)
                   .FirstOrDefaultAsync(cancellationToken);

                if (award is null || award.ValidUntil < DateTime.UtcNow)
                    return NemesisResultFactory
                       .Failure<NemesisBetWinRollbackResponse>(NemesisErrorCode.InappropriateArgument);
            }

            var walletResult = await _walletService.WinAsync(
                request.SessionToken,
                request.RoundId,
                request.TransactionId,
                NemesisMoneyHelper.ToBalance(request.Amount, request.Currency),
                request.RoundClosed,
                request.Currency,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToNemesisFailureResult<NemesisBetWinRollbackResponse>();
            var data = walletResult.Data;

            if (isAward)
            {
                var awardRound = new AwardRound
                {
                    AwardId = request.BonusCode!,
                    RoundId = data.Round.Id
                };
                _context.Add(awardRound);
                await _context.SaveChangesAsync(cancellationToken);
            }

            var response = new NemesisBetWinRollbackResponse(
                data.Transaction.InternalId,
                data.Transaction.Id,
                NemesisMoneyHelper.FromBalance(data.Balance, data.Currency, out var multiplier),
                data.Currency,
                multiplier);

            return NemesisResultFactory.Success(response);
        }
    }
}