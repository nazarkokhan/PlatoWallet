namespace Platipus.Wallet.Api.Application.Requests.Wallets.Evoplay;

using Base;
using Results.Evoplay;

public sealed record EvoplayBalanceRequest(
        string Token, 
        string PlayerId) : IEvoplayRequest, IRequest<IEvoplayResult>;