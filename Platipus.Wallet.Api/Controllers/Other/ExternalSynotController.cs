namespace Platipus.Wallet.Api.Controllers.Other;

using Abstract;
using Application.Responses.Synot.Base;
using Application.Services.SynotGameApi.External;
using Application.Services.SynotGameApi.Models;
using Application.Services.SynotGameApi.Requests;
using Domain.Entities.Enums;
using Extensions;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.ControllerSpecificJsonOptions;

[Route("external/synot")]
[JsonSettingsName(WalletProvider.Synot)]
[ProducesResponseType(typeof(SynotErrorResponse), StatusCodes.Status400BadRequest)]
public sealed class ExternalSynotController : RestApiController
{
    private readonly IMediator _mediator;

    public ExternalSynotController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet("getGames")]
    [ProducesResponseType(typeof(List<SynotGameModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetGames(
        [FromQuery] SynotGetGamesRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("launch")]
    [ProducesResponseType(typeof(SynotGetGameLaunchScriptGameApiRequest), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLaunchGameScript(
        [FromBody] SynotGetGameLaunchScriptRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}