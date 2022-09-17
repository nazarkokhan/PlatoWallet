namespace PlatipusWallet.Api.Application.Requests.Responses;

using PlatipusWallet.Api.Results.External;
using PlatipusWallet.Api.Results.External.Enums;

public record BalanceResponse(
    Status Status,
    string Balance) : BaseResponse(Status);