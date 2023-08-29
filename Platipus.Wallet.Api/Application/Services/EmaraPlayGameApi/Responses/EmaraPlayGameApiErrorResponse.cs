namespace Platipus.Wallet.Api.Application.Services.EmaraPlayGameApi.Responses;

public sealed record EmaraPlayGameApiErrorResponse(
    string Error,
    string Description);