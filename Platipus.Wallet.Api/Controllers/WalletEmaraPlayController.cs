namespace Platipus.Wallet.Api.Controllers;

using Abstract;
using Application.Requests.Wallets.EmaraPlay;
using Application.Requests.Wallets.EmaraPlay.Base;
using Domain.Entities.Enums;
using Extensions;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.ControllerSpecificJsonOptions;
using StartupSettings.Filters;

[Route("wallet/emaraplay")]
[MockedErrorActionFilter(Order = 1)]
[EmaraPlayVerifyHashFilter(Order = 2)]
[JsonSettingsName(nameof(CasinoProvider.EmaraPlay))]
[ProducesResponseType(typeof(EmaraPlayBaseResponse), StatusCodes.Status400BadRequest)]
public class WalletEmaraPlayController : RestApiController
{
    private readonly IMediator _mediator;

    public WalletEmaraPlayController(IMediator mediator) => _mediator = mediator;

    [HttpPost("balance")]
    [ProducesResponseType(typeof(EmaraPlayBaseResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Balance(
        EmaraPlayBalanceRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("bet")]
    [ProducesResponseType(typeof(EmaraPlayBaseResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Bet(
        EmaraPlayBetRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("win")]
    [ProducesResponseType(typeof(EmaraPlayBaseResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Result(
        EmaraPlayResultRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("rollback")]
    [ProducesResponseType(typeof(EmaraPlayBaseResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Refund(
        EmaraPlayRefundRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}