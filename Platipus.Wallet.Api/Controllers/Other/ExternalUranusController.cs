namespace Platipus.Wallet.Api.Controllers.Other;

using Abstract;
using Application.Requests.Wallets.Uranus.Base;
using Application.Requests.Wallets.Uranus.Data;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.ControllerSpecificJsonOptions;
using StartupSettings.Filters.Security.Uranus;

[Route("external/uranus/game")]
[ServiceFilter(typeof(UranusSecurityFilter), Order = 1)]
[JsonSettingsName(nameof(CasinoProvider.Uranus))]
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
        [FromBody] GetLaunchGameUrlRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("demo")]
    [ProducesResponseType(typeof(UranusSuccessResponse<UranusGameUrlData>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDemoLaunchGameUrl(
        [FromBody] GetDemoLaunchGameUrlRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}