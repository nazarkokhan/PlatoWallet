namespace Platipus.Wallet.Api.Application.Services.SynotGameApi.External;

using Models;

public sealed record SynotGetGamesResponse(List<SynotGameModel> Games);