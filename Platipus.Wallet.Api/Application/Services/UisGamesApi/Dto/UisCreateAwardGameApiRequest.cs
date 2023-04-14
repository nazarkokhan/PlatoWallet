namespace Platipus.Wallet.Api.Application.Services.UisGamesApi.Dto;

public record UisCreateAwardGameApiRequest(
    string Login,
    string Password,
    string GameId,
    string Quantity,
    string ValidUntil,
    string RequestSign,
    string Env);