namespace Platipus.Wallet.Api.Application.Services.UisGamesApi.Dto.Response;

public record UisGameApiErrorResponse(
    string StatusCode,
    string Error,
    string Message);