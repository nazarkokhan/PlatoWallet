namespace PlatipusWallet.Api.Application.Requests.DTOs;

using PlatipusWallet.Api.Application.Requests.Base.Responses;

public record GetCurrencyDto(
    Guid Id, 
    string Name) : BaseResponse;