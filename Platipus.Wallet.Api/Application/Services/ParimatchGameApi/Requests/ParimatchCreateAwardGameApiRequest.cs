namespace Platipus.Wallet.Api.Application.Services.ParimatchGameApi.Requests;

using JetBrains.Annotations;

[PublicAPI]
public record ParimatchCreateAwardGameApiRequest(
    string Cid,
    string PlayerId,
    string Currency,
    string[] Games,
    int? Count,
    int? Bet,
    string EndDate);