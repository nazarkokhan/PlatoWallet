namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Results;

using Base;

public sealed record RoundDetailsResult(
    string Type, 
    string Details, 
    string Complete) : IEmaraPlayBaseResponse;