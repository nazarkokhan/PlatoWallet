namespace Platipus.Wallet.Api.Controllers.Other;

using Abstract;
using Application.Requests.Wallets.Atlas;
using Application.Responses.AtlasPlatform;
using Application.Services.AtlasGameApi.Requests;
using Domain.Entities.Enums;
using Extensions;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.ControllerSpecificJsonOptions;
using StartupSettings.Filters.Security.AtlasPlatform;

[Route("external/atlas")]
[JsonSettingsName(WalletProvider.Atlas)]
public sealed class ExternalAtlasController : RestApiController
{
    private readonly IMediator _mediator;

    public ExternalAtlasController(IMediator mediator) =>
        _mediator = mediator;

    [HttpGet("getGames")]
    [ProducesResponseType(typeof(AtlasGetGamesListResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetGames(
        AtlasGetGamesListRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("assignBonus")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    public async Task<IActionResult> AssignFreeSpinBonus(
        [FromBody] AtlasAssignFreeSpinBonusRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("registerBonus")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    public async Task<IActionResult> RegisterFreeSpinBonus(
        [FromBody] AtlasRegisterFreeSpinBonusRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}