namespace Platipus.Wallet.Api.Controllers.Other;

using Abstract;
using Application.Requests.Wallets.Uranus.Base;
using Application.Requests.Wallets.Uranus.Data;
using Application.Services.UranusGamesApi.External;
using Application.Services.VegangsterGameApi.External;
using Application.Services.VegangsterGameApi.Requests;
using Domain.Entities.Enums;
using Extensions;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.ControllerSpecificJsonOptions;

[Route("wallet/external/vegangster")]
[JsonSettingsName(WalletProvider.Vegangster)]
public sealed class ExternalVegangsterController : RestApiController
{
    private readonly IMediator _mediator;

    public ExternalVegangsterController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("game/list")]
    [ProducesResponseType(typeof(VegangsterGetAvailableGamesResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAvailableGames(
        [FromBody] VegangsterGetAvailableGamesRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("game/url")]
    [ProducesResponseType(typeof(VegangsterGetLaunchUrlResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLaunchUrl(
        [FromBody] VegangsterGetLaunchUrlRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("game/demo/url")]
    [ProducesResponseType(typeof(VegangsterGetLaunchUrlResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDemoLaunchUrl(
        [FromBody] VegangsterGetDemoLaunchUrlRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("freegames/grant")]
    [ProducesResponseType(typeof(VegangsterGrantResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Grant(
        [FromBody] VegangsterGrantRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}