namespace Platipus.Wallet.Api.Application.Requests.Wallets.Evoplay;

using Base;
using Data;
using Results.Uranus.WithData;

public abstract record UranusBaseRequest(
        string SessionToken,
        string PlayerId,
        string TransactionId,
        string Amount,
        string Currency,
        string? Payload)
    : IUranusRequest, IRequest<IUranusResult<UranusSuccessResponse<UranusCommonDataWithTransaction>>>;