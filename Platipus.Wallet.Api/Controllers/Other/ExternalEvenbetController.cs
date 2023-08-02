namespace Platipus.Wallet.Api.Controllers.Other;

using Abstract;
using Application.Requests.Wallets.Evenbet.Models;
using Application.Responses.Evenbet.Base;
using Application.Services.EvenbetGamesApi.External;
using Domain.Entities.Enums;
using Extensions;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.ControllerSpecificJsonOptions;
using StartupSettings.Filters.Security.Evenbet;

[Route("external/evenbet/game")]
[JsonSettingsName(nameof(CasinoProvider.Evenbet))]
[ProducesResponseType(typeof(EvenbetFailureResponse), StatusCodes.Status200OK)]
public sealed class ExternalEvenbetController : RestApiController
{
    private readonly IMediator _mediator;

    public ExternalEvenbetController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("list")]
    [ProducesResponseType(typeof(List<EvenbetGameModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetGames(
        [FromBody] EvenbetGetGamesRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("launch")]
    [ProducesResponseType(typeof(EvenbetGetLaunchGameUrlResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLaunchGameUrl(
        [FromBody] EvenbetGetLaunchGameUrlRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}