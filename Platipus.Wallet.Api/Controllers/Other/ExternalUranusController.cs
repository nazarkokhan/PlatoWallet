namespace Platipus.Wallet.Api.Controllers.Other;

using System.ComponentModel.DataAnnotations;
using Abstract;
using Application.Requests.Wallets.Uranus.Base;
using Application.Requests.Wallets.Uranus.Data;
using Application.Services.UranusGamesApi.External;
using Domain.Entities.Enums;
using Extensions;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.ControllerSpecificJsonOptions;
using StartupSettings.Filters.Security.Uranus;

[Route("external/uranus")]
[ServiceFilter(typeof(UranusExternalSecurityFilter), Order = 1)]
[JsonSettingsName(WalletProvider.Uranus)]
[ProducesResponseType(typeof(UranusFailureResponse), StatusCodes.Status200OK)]
public sealed class ExternalUranusController : RestApiController
{
    private readonly IMediator _mediator;

    public ExternalUranusController(IMediator mediator) =>
        _mediator = mediator;

    [HttpPost("game/list")]
    [ProducesResponseType(typeof(UranusSuccessResponse<UranusAvailableGamesData>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAvailableGames(
        [FromBody] UranusGetAvailableGamesRequest request,
        [Required][FromHeader(Name = UranusHeaders.XSignature)] string xSignature,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("game/launch")]
    [ProducesResponseType(typeof(UranusSuccessResponse<UranusGameUrlData>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLaunchGameUrl(
        [FromBody] UranusGetLaunchGameUrlRequest request,
        [Required][FromHeader(Name = UranusHeaders.XSignature)] string xSignature,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
    
    [HttpPost("game/demo")]
    [ProducesResponseType(typeof(UranusSuccessResponse<UranusGameUrlData>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDemoLaunchGameUrl(
        [FromBody] UranusGetDemoLaunchGameUrlRequest request,
        [Required][FromHeader(Name = UranusHeaders.XSignature)] string xSignature,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
    
    [HttpPost("freespin/create")]
    [ProducesResponseType(typeof(UranusSuccessResponse<string[]>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateCampaign(
        [FromBody] UranusCreateCampaignRequest request,
        [Required][FromHeader(Name = UranusHeaders.XSignature)] string xSignature,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
    
    [HttpPost("freespin/cancel")]
    [ProducesResponseType(typeof(UranusSuccessResponse<string[]>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CancelCampaign(
        [FromBody] UranusCancelCampaignRequest request,
        [Required][FromHeader(Name = UranusHeaders.XSignature)] string xSignature,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}