namespace Platipus.Wallet.Api.Application.Services.VegangsterGameApi.External;

using Models;

public sealed record VegangsterGetAvailableGamesResponse(List<VegangsterGameModel> Games);