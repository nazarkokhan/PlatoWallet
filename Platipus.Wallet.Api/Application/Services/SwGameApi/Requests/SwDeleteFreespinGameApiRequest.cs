namespace Platipus.Wallet.Api.Application.Services.SwGameApi.Requests;

using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;

public record SwDeleteFreespinGameApiRequest(
    [property: BindProperty(Name = "key"), DefaultValue("platipus_sw-USD")] string Key,
    [property: BindProperty(Name = "freespin_id")] string FreespinId);