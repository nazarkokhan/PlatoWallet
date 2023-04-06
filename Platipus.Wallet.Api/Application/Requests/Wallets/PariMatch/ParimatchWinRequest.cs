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
using static Results.PariMatch.ParimatchResultFactory;
public record ParimatchWinRequest(
    string Cid,
    string PlayerId,
    string Productid,
    string TxId,
    string RoundId,
    bool RoundClosed,
    int Amount,
    string Currency) : IRequest<IParimatchResult<ParimatchBaseResponse>>, IPariMatchRequest
{

    public class Handler : IRequestHandler<ParimatchWinRequest, IParimatchResult<ParimatchBaseResponse>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<IParimatchResult<ParimatchBaseResponse>> Handle(
            ParimatchWinRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _wallet.WinAsync(
                request.SessionToken, //TODO refactor walletservice
                request.RoundId,
                request.TxId,
                request.Amount,
                request.RoundClosed,
                request.Currency,
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

