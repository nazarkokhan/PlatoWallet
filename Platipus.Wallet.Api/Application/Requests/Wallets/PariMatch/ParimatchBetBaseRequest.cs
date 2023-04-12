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

public record ParimatchBetBaseRequest(
    string Cid,
    string SessionToken,
    string PlayerId,
    string ProductId,
    string TxId,
    string RoundId,
    bool RoundClosed,
    int Amount,
    string Currency) : IRequest<IParimatchResult<ParimatchBaseResponse>>, IPariMatchBaseRequest
{
    public class Handler : IRequestHandler<ParimatchBetBaseRequest, IParimatchResult<ParimatchBaseResponse>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<IParimatchResult<ParimatchBaseResponse>> Handle(
            ParimatchBetBaseRequest baseRequest,
            CancellationToken cancellationToken)
        {
            var walletResult = await _wallet.BetAsync(
                baseRequest.SessionToken,
                baseRequest.RoundId,
                baseRequest.TxId,
                baseRequest.Amount,
                baseRequest.Currency,
                roundFinished: baseRequest.RoundClosed,
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