namespace Platipus.Wallet.Api.Application.Requests.Wallets.Softswiss.Base.Response;

using Requests.Base;

public record SoftswissBalanceResponse(long Balance) : BaseResponse;