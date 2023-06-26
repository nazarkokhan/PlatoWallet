namespace Platipus.Wallet.Api.Controllers.Other;

using Abstract;
using Application.Requests.Wallets.Atlas;
using Application.Responses.AtlasPlatform;
using Application.Services.AtlasGamesApi.Requests;
using Domain.Entities.Enums;
using Extensions;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.ControllerSpecificJsonOptions;
using StartupSettings.Filters.Security.AtlasPlatform;

[Route("external/atlas")]
[ServiceFilter(typeof(AtlasExternalSecurityFilter), Order = 1)]
[JsonSettingsName(nameof(CasinoProvider.Atlas))]
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
}