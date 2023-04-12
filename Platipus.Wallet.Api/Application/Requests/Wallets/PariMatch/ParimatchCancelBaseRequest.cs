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

public record ParimatchCancelBaseRequest(
    string Cid,
    string PlayerId,
    string ProductId,
    string TxId,
    string RoundId,
    int Amount,
    string Currency) : IRequest<IParimatchResult<ParimatchBaseResponse>>, IPariMatchBaseRequest
{


    public class Handler : IRequestHandler<ParimatchCancelBaseRequest, IParimatchResult<ParimatchBaseResponse>>
    {
        private readonly IWalletService _wallet;
        private readonly WalletDbContext _dbContext;


        public Handler(IWalletService wallet,
            WalletDbContext dbContext)
        {
            _wallet = wallet;
            _dbContext = dbContext;
        }

        public async Task<IParimatchResult<ParimatchBaseResponse>> Handle(
            ParimatchCancelBaseRequest baseRequest,
            CancellationToken cancellationToken)
        {
            var user = _dbContext.Set<User>().FirstOrDefault(u => u.Id == int.Parse(baseRequest.PlayerId));

            if (user is null)
            {
                return Failure<ParimatchBaseResponse>(ParimatchErrorCode.LockedPlayer);
            }

            var walletResult = await _wallet.RollbackAsync(
                user.Username,
                baseRequest.TxId,
                baseRequest.RoundId,
                true,
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