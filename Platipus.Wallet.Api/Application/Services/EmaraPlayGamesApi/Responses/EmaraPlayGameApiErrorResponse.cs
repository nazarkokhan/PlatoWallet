namespace Platipus.Wallet.Api.Application.Services.EmaraPlayGamesApi.Responses;

public sealed record EmaraPlayGameApiErrorResponse(
    string Error,
    string Description);