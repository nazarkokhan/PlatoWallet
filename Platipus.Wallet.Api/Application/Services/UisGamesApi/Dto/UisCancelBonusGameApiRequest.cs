namespace Platipus.Wallet.Api.Application.Services.UisGamesApi.Dto;

public record UisCancelBonusGameApiRequest(
    string Login,
    string Password,
    string BonusId,
    string RequestSign,
    string Env);