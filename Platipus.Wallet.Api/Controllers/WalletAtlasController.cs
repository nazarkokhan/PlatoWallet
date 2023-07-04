namespace Platipus.Wallet.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using Application.Responses.AtlasPlatform;
using Abstract;
using Application.Requests.Wallets.Atlas;
using Application.Requests.Wallets.Atlas.Base;
using Extensions;
using StartupSettings.ControllerSpecificJsonOptions;
using StartupSettings.Filters.NewFilterStyle;
using StartupSettings.Filters.Security.AtlasPlatform;
using Domain.Entities.Enums;

[Route("wallet/atlas/")]
[ServiceFilter(typeof(AtlasMockedErrorActionFilter), Order = 1)]
[ServiceFilter(typeof(AtlasSecurityFilter), Order = 2)]
[JsonSettingsName(nameof(CasinoProvider.Atlas))]
[ProducesResponseType(typeof(AtlasErrorResponse), StatusCodes.Status200OK)]
public sealed class WalletAtlasController : RestApiController
{
    private readonly IMediator _mediator;

    public WalletAtlasController(IMediator mediator) => 
        _mediator = mediator;
    
    
    /// <summary>
    ///     This method returns a current balance of the player.
    ///     Client balance must be requested by GP while client initiates game start ,
    ///     and it should be shown in the game iframe.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>
    ///     <see cref="AtlasCommonResponse"/>.
    /// </returns>
    [HttpPost("wallet")]
    [ProducesResponseType(typeof(AtlasCommonResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AtlasErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetClientBalance(
        [FromBody] AtlasGetClientBalanceRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();


    /// <summary>
    ///     This method withdraws the amount from the player’s wallet.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>
    ///     <see cref="AtlasCommonResponse"/>.
    /// </returns>
    [HttpPost("bet:place")]
    [ProducesResponseType(typeof(AtlasCommonResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AtlasErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Bet(
        [FromBody] AtlasBetRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
    
    
    /// <summary>
    ///     This method deposits the amount to the player’s wallet.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>
    ///     <see cref="AtlasCommonResponse"/>.
    /// </returns>
    [HttpPost("bet:win")]
    [ProducesResponseType(typeof(AtlasCommonResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AtlasErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Win(
        [FromBody] AtlasWinRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
    
    
    /// <summary>
    ///     This method should be used to refund a bet amount to the client balance. 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>
    ///     <see cref="AtlasCommonResponse"/>.
    /// </returns>
    [HttpPost("bet:refund")]
    [ProducesResponseType(typeof(AtlasCommonResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AtlasErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refund(
        [FromBody] AtlasRefundRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}