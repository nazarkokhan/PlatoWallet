namespace Platipus.Wallet.Api.Application.Services.EmaraPlayGameApi.Requests;

public sealed record EmaraplayCancelGameApiRequest(
    string Ref,
    string Operator);