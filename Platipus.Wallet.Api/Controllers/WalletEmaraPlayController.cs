using Platipus.Wallet.Api.Controllers.Abstract;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Base;
using Platipus.Wallet.Domain.Entities.Enums;
using Platipus.Wallet.Api.Extensions;
using Microsoft.AspNetCore.Mvc;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Responses;
using Platipus.Wallet.Api.StartupSettings.ControllerSpecificJsonOptions;

namespace Platipus.Wallet.Api.Controllers;

[Route("wallet/emara-play/")]
//[ServiceFilter(typeof(EmaraPlayMockedErrorActionFilter), Order = 1)]
//[ServiceFilter(typeof(EmaraPlaySecurityFilter), Order = 2)]
[JsonSettingsName(nameof(CasinoProvider.EmaraPlay))]
[ProducesResponseType(typeof(EmaraPlayErrorResponse), StatusCodes.Status200OK)]
public sealed class WalletEmaraPlayController : RestApiController
{
    private readonly IMediator _mediator;

    public WalletEmaraPlayController(IMediator mediator) => _mediator = mediator;

    
    [HttpPost("authenticate")]
    [ProducesResponseType(typeof(EmaraPlayBaseResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Authenticate(
        EmaraPlayAuthenticateRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
    
    
    [HttpPost("balance")]
    [ProducesResponseType(typeof(EmaraPlayBalanceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(EmaraPlayBalanceResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Balance(
        EmaraPlayBalanceRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("bet")]
    [ProducesResponseType(typeof(EmaraPlayBetResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Bet(
        EmaraPlayBetRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
    
    [HttpPost("refund")]
    [ProducesResponseType(typeof(EmaraPlayRefundResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Refund(
        EmaraPlayRefundRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
    
    [HttpPost("result")]
    [ProducesResponseType(typeof(EmaraPlayResultResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Result(
        EmaraPlayResultRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
    
    /// <summary>
    ///     Creates a launcher URL. 
    /// </summary>
    /// <param name="urlRequest"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>
    ///     <see cref="EmaraPlayGetLauncherUrlResponse"/>.
    /// </returns>
    [HttpPost("launcher-url")]
    [ProducesResponseType(typeof(EmaraPlayGetLauncherUrlResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLauncherUrl(
        [FromBody] EmaraPlayGetLauncherUrlRequest urlRequest,
        CancellationToken cancellationToken)
        => (await _mediator.Send(urlRequest, cancellationToken)).ToActionResult();
    
    
    /// <summary>
    ///     Gets round's details.
    /// </summary>
    /// <param name="urlRequest"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>
    ///     <see cref="EmaraPlayGetRoundDetailsResponse"/>.
    /// </returns>
    [HttpPost("round-details")]
    [ProducesResponseType(typeof(EmaraPlayGetRoundDetailsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRoundDetails(
        [FromBody] EmaraPlayGetRoundDetailsRequest urlRequest,
        CancellationToken cancellationToken)
        => (await _mediator.Send(urlRequest, cancellationToken)).ToActionResult();
}

