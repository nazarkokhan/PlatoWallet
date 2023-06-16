using Microsoft.AspNetCore.Mvc;
using Platipus.Wallet.Api.Application.Requests.Wallets.AtlasPlatform;
using Platipus.Wallet.Api.Application.Requests.Wallets.AtlasPlatform.Base;
using Platipus.Wallet.Api.Application.Responses.AtlasPlatform;
using Platipus.Wallet.Api.Controllers.Abstract;
using Platipus.Wallet.Api.Extensions;
using Platipus.Wallet.Api.StartupSettings.ControllerSpecificJsonOptions;
using Platipus.Wallet.Api.StartupSettings.Filters.NewFilterStyle;
using Platipus.Wallet.Api.StartupSettings.Filters.Security.AtlasPlatform;
using Platipus.Wallet.Domain.Entities.Enums;

namespace Platipus.Wallet.Api.Controllers;

[Route("wallet/atlas-platform/")]
[ServiceFilter(typeof(AtlasPlatformMockedErrorActionFilter), Order = 1)]
[ServiceFilter(typeof(AtlasPlatformSecurityFilter), Order = 2)]
[JsonSettingsName(nameof(CasinoProvider.AtlasPlatform))]
[ProducesResponseType(typeof(AtlasPlatformErrorResponse), StatusCodes.Status200OK)]
public sealed class WalletAtlasPlatformController : RestApiController
{
    private readonly IMediator _mediator;

    public WalletAtlasPlatformController(IMediator mediator) => 
        _mediator = mediator;
    
    
    /// <summary>
    ///     This method returns a current balance of the player.
    ///     Client balance must be requested by GP while client initiates game start ,
    ///     and it should be shown in the game iframe.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>
    ///     <see cref="AtlasPlatformCommonResponse"/>.
    /// </returns>
    [HttpPost("client/balance")]
    [ProducesResponseType(typeof(AtlasPlatformCommonResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AtlasPlatformErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetClientBalance(
        [FromBody] AtlasPlatformGetClientBalanceRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
    
    
    /// <summary>
    ///     This method authorizes a client.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("client/auth")]
    [ProducesResponseType(typeof(AtlasPlatformAuthorizationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AtlasPlatformErrorResponse), StatusCodes.Status401Unauthorized)]
    public async ValueTask<IActionResult> Authorize(
        [FromBody] AtlasPlatformAuthorizationRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
    
    
    /// <summary>
    ///     This method withdraws the amount from the player’s wallet.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>
    ///     <see cref="AtlasPlatformCommonResponse"/>.
    /// </returns>
    [HttpPost("bet:place")]
    [ProducesResponseType(typeof(AtlasPlatformCommonResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AtlasPlatformErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Bet(
        [FromBody] AtlasPlatformBetRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
    
    
    /// <summary>
    ///     This method deposits the amount to the player’s wallet.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>
    ///     <see cref="AtlasPlatformCommonResponse"/>.
    /// </returns>
    [HttpPost("bet:win")]
    [ProducesResponseType(typeof(AtlasPlatformCommonResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AtlasPlatformErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Win(
        [FromBody] AtlasPlatformWinRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
    
    
    /// <summary>
    ///     This method should be used to refund a bet amount to the client balance. 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>
    ///     <see cref="AtlasPlatformCommonResponse"/>.
    /// </returns>
    [HttpPost("bet:refund")]
    [ProducesResponseType(typeof(AtlasPlatformCommonResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AtlasPlatformErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refund(
        [FromBody] AtlasPlatformRefundRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();


    /// <summary>
    ///     Using this method AP is able to get the list of casino games available for the integration.
    ///     Get games request is sent with Basic authentication.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="casinoId">Id of the Casino.</param>
    /// <returns>
    ///     <see cref="AtlasPlatformGetGamesResponse"/>.
    /// </returns>
    [HttpPost("games")]
    [ServiceFilter(typeof(AtlasPlatformBasicSecurityFilter), Order = 2)]
    [ProducesResponseType(typeof(AtlasPlatformGetGamesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AtlasPlatformErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetGames(
        [FromBody] AtlasPlatformGetGamesRequest request,
        CancellationToken cancellationToken, 
        string? casinoId = "")
    {
        var requestToSend = request with { CasinoId = casinoId };
        return (await _mediator.Send(requestToSend, cancellationToken)).ToActionResult();
    }


    /// <summary>
    ///     Using this method AP is able to get the link for launching the game.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>
    ///     <see cref="AtlasPlatformLaunchGameResponse"/>.
    /// </returns>
    [HttpPost("gameLaunch")]
    [ServiceFilter(typeof(AtlasPlatformBasicSecurityFilter), Order = 2)]
    [ProducesResponseType(typeof(AtlasPlatformLaunchGameResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AtlasPlatformErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LaunchGame(
        [FromBody] AtlasPlatformLaunchGameRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}