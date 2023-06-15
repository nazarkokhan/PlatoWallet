using Microsoft.AspNetCore.Mvc;
using Platipus.Wallet.Api.Application.Requests.Wallets.AtlasPlatform;
using Platipus.Wallet.Api.Application.Requests.Wallets.AtlasPlatform.Base;
using Platipus.Wallet.Api.Application.Responses.AtlasPlatform;
using Platipus.Wallet.Api.Controllers.Abstract;
using Platipus.Wallet.Api.Extensions;
using Platipus.Wallet.Api.StartupSettings.ControllerSpecificJsonOptions;
using Platipus.Wallet.Api.StartupSettings.Filters.Security;
using Platipus.Wallet.Domain.Entities.Enums;

namespace Platipus.Wallet.Api.Controllers;

[Route("wallet/atlas-platform/")]
//[ServiceFilter(typeof(AtlasPlatformMockedErrorActionFilter), Order = 1)]
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
    ///     <see cref="AtlasPlatformGetClientBalanceResponse"/>.
    /// </returns>
    [HttpPost("client/balance")]
    [ProducesResponseType(typeof(AtlasPlatformGetClientBalanceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AtlasPlatformErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetClientBalance(
        [FromBody] AtlasPlatformGetClientBalanceRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
    
    
    /// <summary>
    ///     
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("client/auth")]
    [ProducesResponseType(typeof(AtlasPlatformAuthorizationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AtlasPlatformErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Authorize(
        [FromBody] AtlasPlatformAuthorizationRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}