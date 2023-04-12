// ReSharper disable NotAccessedPositionalProperty.Global
namespace Platipus.Wallet.Api.Application.Requests.Wallets.PariMatch;

using Base;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.PariMatch;
using Results.PariMatch.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;
using static Results.PariMatch.ParimatchResultFactory;

public record ParimatchPlayerInfoBaseRequest(string Cid, string SessionToken)
    : IRequest<IParimatchResult<ParimatchPlayerInfoResponse>>, IPariMatchInfoRequest
{

    public class Handler : IRequestHandler<ParimatchPlayerInfoBaseRequest, IParimatchResult<ParimatchPlayerInfoResponse>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }


        public async Task<IParimatchResult<ParimatchPlayerInfoResponse>> Handle(
            ParimatchPlayerInfoBaseRequest baseRequest,
            CancellationToken cancellationToken)
        {
            var walletResult = await _wallet.GetBalanceAsync(
                baseRequest.SessionToken,
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

