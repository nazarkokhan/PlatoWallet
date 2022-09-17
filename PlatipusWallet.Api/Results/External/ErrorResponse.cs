namespace PlatipusWallet.Api.Results.External;

using Enums;

public record ErrorResponse(
    Status Status,
    string Error,
    string Description) : BaseResponse(Status);