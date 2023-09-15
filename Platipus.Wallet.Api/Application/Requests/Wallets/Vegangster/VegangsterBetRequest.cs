namespace Platipus.Wallet.Api.Application.Requests.Wallets.Vegangster;

using Base;
using Responses.Vegangster;
using Results.Vegangster.WithData;

public sealed record VegangsterBetRequest(string Token, string PlayerId)
    : IVegangsterRequest, IRequest<IVegangsterResult<VegangsterTransactionResponse>>
{
    public class Handler : IRequestHandler<VegangsterBetRequest, IVegangsterResult<VegangsterTransactionResponse>>
    {
        public async Task<IVegangsterResult<VegangsterTransactionResponse>> Handle(
            VegangsterBetRequest request,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}