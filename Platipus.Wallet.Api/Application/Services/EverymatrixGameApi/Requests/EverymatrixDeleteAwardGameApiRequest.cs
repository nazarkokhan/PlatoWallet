namespace Platipus.Wallet.Api.Application.Services.EverymatrixGameApi.Requests;

using JetBrains.Annotations;

[PublicAPI]
public record EverymatrixDeleteAwardGameApiRequest(
    string UserId,
    string BonusId);