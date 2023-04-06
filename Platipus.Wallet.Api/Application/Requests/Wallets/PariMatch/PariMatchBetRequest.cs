// ReSharper disable NotAccessedPositionalProperty.Global

namespace Platipus.Wallet.Api.Application.Requests.Wallets.PariMatch;

using Domain.Entities;
using Infrastructure.Persistence;
using Results.PariMatch;
using Results.PariMatch.WithData;
using static Results.PariMatch.PariMatchResultFactory;
using Microsoft.EntityFrameworkCore;
using Results.ResultToResultMappers;
using Services.Wallet;
using TODO.PariMatch.Base;

public record PariMatchBetRequest(
    string Cid,
    string SessionToken,
    string PlayerId,
    string ProductId,
    string TxId,
    string RoundId,
    bool RoundClosed,
    int Amount,
    string Currency) : IRequest<IPariMatchResult<ParimatchBaseResponse>>
{
    public class Handler : IRequestHandler<PariMatchBetRequest, IPariMatchResult<ParimatchBaseResponse>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<IPariMatchResult<ParimatchBaseResponse>> Handle(
            PariMatchBetRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _wallet.BetAsync(
                request.SessionToken,
                request.RoundId,
                request.TxId,
                request.Amount,
                request.Currency,
                roundFinished: request.RoundClosed,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToParimatchResult<ParimatchBaseResponse>();
            var data = walletResult.Data;

            var response = new ParimatchBaseResponse(
                data.Transaction.ToString(),
                data.Transaction.ToString(),
                DateTimeOffset.Now,
                (int) data.Balance,
                true);

            return Success<ParimatchBaseResponse>(response);
        }
    }
}