namespace Platipus.Wallet.Api.Controllers.Other;

using Abstract;
using Application.Requests.Wallets.Uranus.Base;
using Application.Requests.Wallets.Uranus.Data;
using Application.Services.UranusGamesApi.External;
using Domain.Entities.Enums;
using Extensions;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.ControllerSpecificJsonOptions;
using StartupSettings.Filters.Security.Uranus;

[Route("external/uranus/game")]
[ServiceFilter(typeof(UranusExternalSecurityFilter), Order = 1)]
[JsonSettingsName(WalletProvider.Uranus)]
[ProducesResponseType(typeof(UranusFailureResponse), StatusCodes.Status200OK)]
public sealed class ExternalUranusController : RestApiController
{
    private readonly IMediator _mediator;

    public ExternalUranusController(IMediator mediator) =>
        _mediator = mediator;

    [HttpPost("list")]
    [ProducesResponseType(typeof(UranusSuccessResponse<UranusAvailableGamesData>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAvailableGames(
        [FromBody] UranusGetAvailableGamesRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("launch")]
    [ProducesResponseType(typeof(UranusSuccessResponse<UranusGameUrlData>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLaunchGameUrl(
        [FromBody] UranusGetLaunchGameUrlRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
    
    [HttpPost("demo")]
    [ProducesResponseType(typeof(UranusSuccessResponse<UranusGameUrlData>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDemoLaunchGameUrl(
        [FromBody] UranusGetDemoLaunchGameUrlRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}