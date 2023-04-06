namespace Platipus.Wallet.Api.Controllers;

using Abstract;
using Application.Requests.Wallets.EmaraPlay;
using Application.Requests.Wallets.TODO.EmaraPlay;
using Application.Requests.Wallets.TODO.EmaraPlay.Base;
using Domain.Entities.Enums;
using Extensions;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.ControllerSpecificJsonOptions;
using StartupSettings.Filters;
using StartupSettings.Filters.Security;

[Route("wallet/emaraplay")]
[MockedErrorActionFilter(Order = 1)]
[EmaraPlayVerifyHashFilter(Order = 2)]
[JsonSettingsName(nameof(CasinoProvider.EmaraPlay))]
[ProducesResponseType(typeof(EmaraPlayResponse), StatusCodes.Status400BadRequest)]
public class WalletEmaraPlayController : RestApiController
{
    private readonly IMediator _mediator;

    public WalletEmaraPlayController(IMediator mediator) => _mediator = mediator;

    [HttpPost("balance")]
    [ProducesResponseType(typeof(EmaraPlayResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Balance(
        EmaraPlayBalanceRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("bet")]
    [ProducesResponseType(typeof(EmaraPlayResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Bet(
        EmaraPlayBetRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("result")]
    [ProducesResponseType(typeof(EmaraPlayResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Result(
        EmaraPlayResultRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("refund")]
    [ProducesResponseType(typeof(EmaraPlayResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Refund(
        EmaraPlayRefundRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}

