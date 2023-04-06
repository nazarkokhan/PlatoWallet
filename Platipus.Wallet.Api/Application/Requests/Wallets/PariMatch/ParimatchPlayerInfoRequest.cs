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

public record ParimatchPlayerInfoRequest(string Cid, string SessionToken)
    : IRequest<IParimatchResult<ParimatchPlayerInfoResponse>>, IPariMatchRequest
{

    public class Handler : IRequestHandler<ParimatchPlayerInfoRequest, IParimatchResult<ParimatchPlayerInfoResponse>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }


        public async Task<IParimatchResult<ParimatchPlayerInfoResponse>> Handle(
            ParimatchPlayerInfoRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _wallet.GetBalanceAsync(
                request.SessionToken,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToParimatchResult<ParimatchPlayerInfoResponse>();
            var data = walletResult.Data;

            var response = new ParimatchPlayerInfoResponse(
                data.UserId.ToString(),
                (int)data.Balance,
                data.Currency,
                "Ukraine",
                data.Username);

            return Success<ParimatchPlayerInfoResponse>(response);
        }
    }
}

