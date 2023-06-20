namespace Platipus.Wallet.Api.Application.Services.EmaraPlayGamesApi.Requests;

public sealed record EmaraplayCancelGameApiRequest(
    string Ref,
    string Operator);