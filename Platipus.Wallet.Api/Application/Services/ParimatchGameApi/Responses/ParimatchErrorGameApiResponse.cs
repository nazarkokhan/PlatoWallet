namespace Platipus.Wallet.Api.Application.Services.ParimatchGameApi.Responses;

using JetBrains.Annotations;

[PublicAPI]
public record ParimatchErrorGameApiResponse(int Error, string Description);