namespace Platipus.Wallet.Api.Application.Services.SwGameApi.Requests;

using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;

public record SwCreateAwardGameApiRequest(
    [property: BindProperty(Name = "key"), DefaultValue("platipus_sw-USD")] string Key,
    [property: BindProperty(Name = "userid")] string UserId,
    [property: BindProperty(Name = "games")] string Games,
    [property: BindProperty(Name = "freespin_id")] string FreespinId,
    [property: BindProperty(Name = "freespin_bet")] double FreespinBet,
    [property: BindProperty(Name = "freespin_amount")] decimal FreespinAmount,
    [property: BindProperty(Name = "expire")] string Expire,
    [property: BindProperty(Name = "launchid")] string LaunchId,
    [property: BindProperty(Name = "token")] string Token);