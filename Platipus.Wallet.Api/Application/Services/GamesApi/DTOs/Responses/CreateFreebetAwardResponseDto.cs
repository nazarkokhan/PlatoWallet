namespace Platipus.Wallet.Api.Application.Services.GamesApi.DTOs.Responses;

using Requests.Wallets.Psw.Base.Response;

public record CreateFreebetAwardResponseDto(string AwardId) : PswBaseResponse;