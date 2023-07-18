namespace Platipus.Wallet.Api.Controllers.Other;

using Abstract;
using Application.Responses.Evenbet.Base;
using Application.Services.EvenbetGamesApi.External;
using Domain.Entities.Enums;
using Extensions;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.ControllerSpecificJsonOptions;
using StartupSettings.Filters.Security.Evenbet;

[Route("external/evenbet/game")]
[ServiceFilter(typeof(EvenbetSecurityFilter), Order = 1)]
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
    [ProducesResponseType(typeof(EvenbetGetGamesResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetGames(
        [FromBody] EvenbetGetGamesRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
    //
    // [HttpPost("launch")]
    // [ProducesResponseType(typeof(EvenbetGetLaunchGameUrlResponse), StatusCodes.Status200OK)]
    // public async Task<IActionResult> GetLaunchGameUrl(
    //     [FromBody] EvenbetGetLaunchGameUrlRequest request,
    //     CancellationToken cancellationToken)
    //     => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}