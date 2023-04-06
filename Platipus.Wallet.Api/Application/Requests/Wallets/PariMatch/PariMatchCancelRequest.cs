// ReSharper disable IdentifierTypo
// ReSharper disable NotAccessedPositionalProperty.Global

namespace Platipus.Wallet.Api.Application.Requests.Wallets.PariMatch;

using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.PariMatch;
using Results.PariMatch.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;
using TODO.PariMatch.Base;
using static Results.PariMatch.PariMatchResultFactory;

public record PariMatchCancelRequest(
    string Cid,
    string PlayerId,
    string ProductId,
    string TxId,
    string RoundId,
    int Amount,
    string Currency) : IRequest<IPariMatchResult<ParimatchBaseResponse>>
{


    public class Handler : IRequestHandler<PariMatchCancelRequest, IPariMatchResult<ParimatchBaseResponse>>
    {
        private readonly IWalletService _wallet;


        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<IPariMatchResult<ParimatchBaseResponse>> Handle(
            PariMatchCancelRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _wallet.RollbackAsync(
                request.SessionId,
                request.TxId,
                request.RoundId,
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