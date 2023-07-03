namespace Platipus.Wallet.Api.Application.Requests.Wallets.Evoplay;

using Base;
using Results.Evoplay;

public sealed record EvoplayBalanceRequest(
        string SessionToken, 
        string PlayerId) : IEvoplayRequest, IRequest<IEvoplayResult>;