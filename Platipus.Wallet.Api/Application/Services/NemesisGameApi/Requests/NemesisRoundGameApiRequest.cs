namespace Platipus.Wallet.Api.Application.Services.NemesisGameApi.Requests;

using JetBrains.Annotations;

[PublicAPI]
public record NemesisRoundGameApiRequest(string PlayerId, string TransactionId, string RoundId);