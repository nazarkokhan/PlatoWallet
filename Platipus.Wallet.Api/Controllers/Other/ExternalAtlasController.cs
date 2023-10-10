namespace Platipus.Wallet.Api.Controllers.Other;

using System.ComponentModel.DataAnnotations;
using Abstract;
using Application.Requests.Wallets.Atlas;
using Application.Responses.AtlasPlatform;
using Domain.Entities.Enums;
using Extensions;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.ControllerSpecificJsonOptions;

[Route("external/atlas")]
[JsonSettingsName(WalletProvider.Atlas)]
public sealed class ExternalAtlasController : RestApiController
{
    private readonly IMediator _mediator;

    public ExternalAtlasController(IMediator mediator) =>
        _mediator = mediator;

    [HttpPost("gameLaunch")]
    [ProducesResponseType(typeof(Uri), StatusCodes.Status200OK)]
    public async Task<IActionResult> GameLaunch(
        [FromBody] AtlasGameLaunchRequest request,
        CancellationToken cancellationToken) => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("getGames")]
    [ProducesResponseType(typeof(AtlasGetGamesListResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetGames(
        [Required] [PublicAPI, FromHeader(Name = AtlasHeaders.AuthorizationToken)] string authToken,
        [FromBody] AtlasGetGamesListRequest request,
        CancellationToken cancellationToken)
    {
        var requestToProcess = request with { Token = authToken };

        var result = await _mediator.Send(requestToProcess, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("assignBonus")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    public async Task<IActionResult> AssignFreeSpinBonus(
        [Required] [PublicAPI, FromHeader(Name = AtlasHeaders.AuthorizationToken)] string authToken,
        [FromBody] AtlasAssignFreeSpinBonusRequest request,
        CancellationToken cancellationToken)
    {
        var requestToProcess = request with { Token = authToken };

        var result = await _mediator.Send(requestToProcess, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("registerBonus")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    public async Task<IActionResult> RegisterFreeSpinBonus(
        [Required] [PublicAPI, FromHeader(Name = AtlasHeaders.AuthorizationToken)] string authToken,
        [FromBody] AtlasRegisterFreeSpinBonusRequest request,
        CancellationToken cancellationToken)
    {
        var requestToProcess = request with { Token = authToken };

        var result = await _mediator.Send(requestToProcess, cancellationToken);
        return result.ToActionResult();
    }
}