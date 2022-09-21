namespace PlatipusWallet.Api.Application.Requests.Responses;

using Base;
using PlatipusWallet.Api.Results.External.Enums;

public record BalanceResponse(
    Status Status,
    decimal Balance) : BaseResponse(Status);