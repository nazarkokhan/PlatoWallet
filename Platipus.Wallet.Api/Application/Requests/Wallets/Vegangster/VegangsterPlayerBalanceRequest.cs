namespace Platipus.Wallet.Api.Application.Requests.Wallets.Vegangster;

using Base;
using Responses.Vegangster;
using Results.Vegangster.WithData;

public sealed record VegangsterPlayerBalanceRequest(string Token, string PlayerId)
    : IVegangsterRequest, IRequest<IVegangsterResult<VegangsterPlayerBalanceResponse>>
{
    public sealed class Handler
        : IRequestHandler<VegangsterPlayerBalanceRequest, IVegangsterResult<VegangsterPlayerBalanceResponse>>
    {
        public async Task<IVegangsterResult<VegangsterPlayerBalanceResponse>> Handle(VegangsterPlayerBalanceRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}