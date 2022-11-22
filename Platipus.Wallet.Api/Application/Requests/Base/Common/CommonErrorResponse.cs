namespace Platipus.Wallet.Api.Application.Requests.Base.Common;

using Results.Common;

public record CommonErrorResponse(ErrorCode ErrorCode) : BaseResponse;