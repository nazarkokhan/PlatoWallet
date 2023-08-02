namespace Platipus.Wallet.Api.Controllers;

using Abstract;
using Application.Requests.Wallets.EmaraPlay;
using Application.Requests.Wallets.EmaraPlay.Base;
using Domain.Entities.Enums;
using Extensions;
using Microsoft.AspNetCore.Mvc;
using Application.Requests.Wallets.EmaraPlay.Responses;
using StartupSettings.ControllerSpecificJsonOptions;
using StartupSettings.Filters.NewFilterStyle;
using StartupSettings.Filters.Security;


[Route("wallet/emara-play")]
[ServiceFilter(typeof(EmaraPlayMockedErrorActionFilter), Order = 1)]
[ServiceFilter(typeof(EmaraPlaySecurityFilter), Order = 2)]
[JsonSettingsName(CasinoProvider.EmaraPlay)]
[ProducesErrorResponseType(typeof(EmaraPlayErrorResponse))]
public sealed class WalletEmaraPlayController : RestApiController
{
    private readonly IMediator _mediator;

    public WalletEmaraPlayController(IMediator mediator) => _mediator = mediator;

    [HttpPost("authenticate")]
    [ProducesResponseType(typeof(EmaraplayAuthenticateResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Authenticate(
        [FromBody] EmaraPlayAuthenticateRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("balance")]
    [ProducesResponseType(typeof(EmaraPlayBalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Balance(
        [FromBody] EmaraPlayBalanceRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("bet")]
    [ProducesResponseType(typeof(EmaraPlayBetResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Bet(
        [FromBody] EmaraPlayBetRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("refund")]
    [ProducesResponseType(typeof(EmaraPlayRefundResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Refund(
        [FromBody] EmaraPlayRefundRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("result")]
    [ProducesResponseType(typeof(EmaraPlayResultResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Result(
        [FromBody] EmaraPlayResultRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
    
}