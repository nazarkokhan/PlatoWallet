namespace PlatipusWallet.Api.Application.Requests.DTOs;

using Base.Responses;

public record GetCurrencyDto(
    Guid Id, 
    string Name) : BaseResponse;