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
[ServiceFilter(typeof(AtlasExternalSecurityFilter), Order = 1)]
[JsonSettingsName(WalletProvider.Atlas)]
public sealed class ExternalAtlasController : RestApiController
{
    private readonly IMediator _mediator;

    public ExternalAtlasController(IMediator mediator) => 
        _mediator = mediator;

    /// <summary>
    ///     Using this method AP is able to get the list of casino games available for the integration.
    ///     Get games request is sent with Basic authentication.
    /// </summary>
    /// <param name="environment">Environment for work.</param>
    /// <param name="cancellationToken"></param>
    /// <param name="casinoId">Id of the Casino.</param>
    [HttpGet("getGames")]
    [ProducesResponseType(typeof(AtlasGetGamesListResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetGames(
        string environment,
        CancellationToken cancellationToken,
        [FromQuery]string? casinoId = null)
    {
        var authHeader = Request.Headers["Authorization"].SingleOrDefault();
        var token = authHeader?["Basic ".Length..];
        var requestToSend = new AtlasGetGamesListRequest(
            environment, new AtlasGetGamesListGameApiRequest(casinoId), token);
        return (await _mediator.Send(requestToSend, cancellationToken)).ToActionResult();
    }

    /// <summary>
    ///     Using this method Atlas Platform can add one player to the existing FreeSpins Bonus.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    [HttpPost("assignBonus")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    public async Task<IActionResult> AssignFreeSpinBonus(
       [FromBody]AtlasAssignFreeSpinBonusRequest request,
        CancellationToken cancellationToken)
    {
        var authHeader = Request.Headers["Authorization"].SingleOrDefault();
        var token = authHeader?["Basic ".Length..];
        var requestToSend = request with { Token = token! };
        return (await _mediator.Send(requestToSend, cancellationToken)).ToActionResult();
    }

    /// <summary>
    ///     Using this method Atlas Platform registers a new FreeSpins Bonus within the Game Provider system.
    ///     BonusId is a unique identifier.
    ///     The Game Provider should not create new FreeSpins bonus if there is an active FreeSpins bonus with the same bonusId.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    [HttpPost("registerBonus")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    public async Task<IActionResult> RegisterFreeSpinBonus(
        [FromBody]AtlasRegisterFreeSpinBonusRequest request,
        CancellationToken cancellationToken)
    {
        var authHeader = Request.Headers["Authorization"].SingleOrDefault();
        var token = authHeader?["Basic ".Length..];
        var requestToSend = request with { Token = token! };
        return (await _mediator.Send(requestToSend, cancellationToken)).ToActionResult();
    }
}