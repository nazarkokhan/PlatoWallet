namespace PlatipusWallet.Api.Results.External;

using Application.Requests.Base.Requests;
using Application.Requests.Base.Responses;
using Enums;

public record ErrorResponse(
    Status Status,
    int Error,
    string Description) : BaseResponse(Status);