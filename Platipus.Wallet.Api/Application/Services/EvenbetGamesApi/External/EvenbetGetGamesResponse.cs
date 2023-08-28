﻿namespace Platipus.Wallet.Api.Application.Services.EvenbetGamesApi.External;

using System.Text.Json.Serialization;
using Platipus.Wallet.Api.Application.Requests.Wallets.Evenbet.Models;

public sealed record EvenbetGetGamesResponse([property: JsonPropertyName("games")] List<EvenbetGameModel> Games);