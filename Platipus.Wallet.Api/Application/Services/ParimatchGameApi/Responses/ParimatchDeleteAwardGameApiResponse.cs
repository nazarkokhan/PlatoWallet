namespace Platipus.Wallet.Api.Application.Services.ParimatchGameApi.Responses;

using JetBrains.Annotations;

[PublicAPI]
public record ParimatchDeleteAwardGameApiResponse(int Error, string Description);