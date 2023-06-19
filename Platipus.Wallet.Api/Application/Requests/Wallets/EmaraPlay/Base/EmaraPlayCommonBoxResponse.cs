using Platipus.Wallet.Api.Application.Results.EmaraPlay;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Base;

public record EmaraPlayCommonBoxResponse<TResult>(TResult Result)
    : EmaraPlayBaseResponse(EmaraPlayErrorCode.Success)
    where TResult : IEmaraPlayBaseResponse;