namespace PlatipusWallet.Api.Results.External;

using Application.Requests.Base;
using Enums;

public record ErrorResponse(
    Status Status,
    int Error,
    string Description) : BaseResponse(Status);