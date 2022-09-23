namespace PlatipusWallet.Api.Application.Requests.Base.Responses;

using Requests;

public record BalanceResponse(decimal Balance) : BaseResponse;