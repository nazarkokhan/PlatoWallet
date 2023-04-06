// ReSharper disable NotAccessedPositionalProperty.Global

namespace Platipus.Wallet.Api.Application.Requests.Wallets.PariMatch;

using Domain.Entities;
using Infrastructure.Persistence;
using Results.PariMatch;
using Results.PariMatch.WithData;
using static Results.PariMatch.ParimatchResultFactory;
using Microsoft.EntityFrameworkCore;
using Results.ResultToResultMappers;
using Services.Wallet;
using TODO.PariMatch.Base;

public record ParimatchBetRequest(
    string Cid,
    string SessionToken,
    string PlayerId,
    string ProductId,
    string TxId,
    string RoundId,
    bool RoundClosed,
    int Amount,
    string Currency) : IRequest<IParimatchResult<ParimatchBaseResponse>>, IPariMatchRequest
{
    public class Handler : IRequestHandler<ParimatchBetRequest, IParimatchResult<ParimatchBaseResponse>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<IParimatchResult<ParimatchBaseResponse>> Handle(
            ParimatchBetRequest request,
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