namespace Platipus.Wallet.Api.Application.Requests.Wallets.Evoplay;

using Base;
using Data;
using Results.Evoplay.WithData;

public abstract record EvoplayBaseRequest(
        string SessionToken,
        string PlayerId,
        string TransactionId,
        string Amount,
        string Currency,
        string? Payload)
    : IEvoplayRequest, IRequest<IEvoplayResult<EvoplaySuccessResponse<EvoplayCommonDataWithTransaction>>>;