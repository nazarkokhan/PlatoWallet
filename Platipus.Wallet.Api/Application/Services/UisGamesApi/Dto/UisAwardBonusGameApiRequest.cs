namespace Platipus.Wallet.Api.Application.Services.UisGamesApi.Dto;

public record UisAwardBonusGameApiRequest(
    string Login,
    string Password,
    string Games,
    string Quantity,
    string ValidUntil,
    string RequestSign,
    string Env);