namespace Platipus.Wallet.Api.Application.Requests.Wallets.Vegangster;

using Base;
using Responses.Vegangster;
using Results.Vegangster.WithData;

public sealed record VegangsterWinRequest(string Token, string PlayerId)
    : IVegangsterRequest, IRequest<IVegangsterResult<VegangsterTransactionResponse>>
{
}