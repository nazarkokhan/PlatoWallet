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

public record PariMatchPlayerInfoRequest(string Cid, string SessionToken)
    : IRequest<IPariMatchResult<PariMatchPlayerInfoResponse>>
{

    public class Handler : IRequestHandler<PariMatchPlayerInfoRequest, IPariMatchResult<PariMatchPlayerInfoResponse>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }


        public async Task<IPariMatchResult<PariMatchPlayerInfoResponse>> Handle(
            PariMatchPlayerInfoRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _wallet.GetBalanceAsync(
                request.SessionToken,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToParimatchResult<PariMatchPlayerInfoResponse>();
            var data = walletResult.Data;

            var response = new PariMatchPlayerInfoResponse(
                data.UserId.ToString(),
                (int)data.Balance,
                data.Currency,
                "Ukraine",
                data.Username);

            return Success<PariMatchPlayerInfoResponse>(response);
        }
    }
}

