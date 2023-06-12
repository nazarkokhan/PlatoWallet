using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Results;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Responses;

public sealed record EmaraPlayRefundResponse(string Error, string Description, RefundResult Result);