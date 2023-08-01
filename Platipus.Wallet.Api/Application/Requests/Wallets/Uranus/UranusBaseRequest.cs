namespace Platipus.Wallet.Api.Application.Requests.Wallets.Uranus;

using Base;
using Data;
using Platipus.Wallet.Api.Application.Results.Uranus.WithData;

//TODO?????
public abstract record UranusBaseRequest(
        string SessionToken,
        string PlayerId,
        string TransactionId,
        string Amount,
        string Currency,
        string? Payload)
    : IUranusRequest, IRequest<IUranusResult<UranusSuccessResponse<UranusCommonDataWithTransaction>>>;